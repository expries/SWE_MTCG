using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Npgsql;
using Npgsql.Schema;
using NpgsqlTypes;

namespace MTCG.Database
{
    public class DatabaseManager
    {
        private NpgsqlConnection Connection { get; set; }
        
        private NpgsqlDataReader _reader;

        private string Host { get; }
        
        private string Username { get; }
        
        private string Password { get; }
        
        private string Database { get; }

        public DatabaseManager(string host, string username, string password, string database)
        {
            Host = host;
            Username = username;
            Password = password;
            Database = database;
            Connection = null;
        }

        public static void MapEnum<TEnum>(string name) where TEnum : struct, Enum
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<TEnum>(name);
        }

        public DatabaseManager GetNewInstance()
        {
            return new DatabaseManager(Host, Username, Password, Database);
        }
        
        public TEntity QueryFirstOrDefault<TEntity>(string query) 
            where TEntity : class, new()
        {
            var cmd = new NpgsqlCommand(query);
            Query(cmd);
            return GetNextRecord<TEntity>();
        }
        
        public TEntity QueryFirstOrDefault<TEntity>(string query, object dataObject) 
            where TEntity : class, new()
        {
            Query(query, dataObject);
            return GetNextRecord<TEntity>();
        }
        
        public TEntity QueryFirstOrDefault<TEntity>(NpgsqlCommand cmd) 
            where TEntity : class, new()
        {
            Query(cmd);
            return GetRecord<TEntity>();
        }
        
        public List<TEntity> Query<TEntity>(string query, int limit = 100) 
            where TEntity : class, new()
        {
            var cmd = new NpgsqlCommand(query);
            Query(cmd);
            return GetRecords<TEntity>(limit);
        }

        public List<TEntity> Query<TEntity>(string query, object dataObject, int limit = 100) 
            where TEntity : class, new()
        {
            Query(query, dataObject);
            return GetRecords<TEntity>(limit);
        }
        
        public List<TEntity> Query<TEntity>(NpgsqlCommand cmd, int limit = 100) 
            where TEntity : class, new()
        {
            Query(cmd);
            return GetRecords<TEntity>(limit);
        }
        
        public void Query(string query)
        {
            var cmd = new NpgsqlCommand(query);
            Query(cmd);
        }
        
        public void Query(string query, object dataObject)
        {
            var cmd = BuildCommand(query, dataObject);
            Query(cmd);
        }

        public void Query(NpgsqlCommand cmd)
        {
            Reconnect();
            cmd.Connection = Connection;
            _reader = cmd.ExecuteReader();
        }

        public int ExecuteNonQuery(string sql, object dataObject)
        {
            var cmd = BuildCommand(sql, dataObject);
            return ExecuteNonQuery(cmd);
        }
        
        public int ExecuteNonQuery(NpgsqlCommand cmd)
        {
            Reconnect();
            cmd.Connection = Connection;
            return cmd.ExecuteNonQuery();
        }

        public TEntity GetNextRecord<TEntity>() where TEntity : class, new()
        {
            if (_reader is null)
            {
                throw new ArgumentException("You first have to execute a query to read records.");
            }
            
            _reader.Read();
            return GetRecord<TEntity>();
        }

        private Dictionary<string, object> GetRecord()
        {
            var columnSchema = _reader.GetColumnSchema();
            var record = new Dictionary<string, object>();

            if (!_reader.IsOnRow)
            {
                return record;
            }
            
            foreach (var column in columnSchema)
            {
                AddColumn(column, record);
            }

            return record;
        }

        private static NpgsqlCommand BuildCommand(string sql, object dataObject)
        {
            var cmd = new NpgsqlCommand(sql);
            var readableProperties = dataObject.GetType()
                .GetProperties()
                .Where(property => property.CanRead);
            
            foreach (var property in readableProperties)
            {
                var value = property.GetValue(dataObject);
                cmd.Parameters.AddWithValue(property.Name, value ?? DBNull.Value);
            }

            return cmd;
        }
        
        private List<TEntity> GetRecords<TEntity>(int limit) where TEntity : class, new()
        {
            var records = new List<TEntity>();
            TEntity record; int i = 0;
            
            while ((record = GetNextRecord<TEntity>()) != null && i < limit)
            {
                records.Add(record);
                i++;
            }

            return records;
        }
        
        private TEntity GetRecord<TEntity>() where TEntity : class, new()
        {
            var record = GetRecord();
            var entity = new TEntity();

            if (record.Count == 0)
            {
                return null;
            }

            var writeableProperties = typeof(TEntity)
                .GetProperties()
                .Where(property => property.CanWrite);

            foreach (var property in writeableProperties)
            {
                var columnAttr = property.GetCustomAttribute(typeof(ColumnAttribute), true) as ColumnAttribute;
                
                // ignore properties without column attribute
                if (columnAttr is null)
                {
                    continue;
                }

                var columnValue = GetColumnValueByName(columnAttr.Name, record);

                // if column is null leave property as is
                if (columnValue is null || columnValue is DBNull) 
                {
                    continue;
                }
                
                property.SetValue(entity, columnValue);
            }

            return entity;
        }

        private static object GetColumnValueByName(string name, IReadOnlyDictionary<string, object> record)
        {
            // find column in record with matching name (case-insensitive)
            var columnNames = record
                .Where(column => string.Equals(column.Key, name, StringComparison.OrdinalIgnoreCase))
                .Select(column => column.Key)
                .ToList();

            if (columnNames.Count == 0)
            {
                return null;
            }

            string columnName = columnNames.First();
            return record[columnName];
        }
        
        private void AddColumn(NpgsqlDbColumn column, IDictionary<string, object> record)
        {
            if (column.ColumnOrdinal is null)
            {
                return;
            }
                
            int ordinal = (int) column.ColumnOrdinal;
            var value = _reader.GetValue(ordinal);
            record.Add(column.ColumnName, value);
        }
        
        private void Connect()
        {
            string connString = $"Host={Host};Username={Username};Password={Password};Database={Database}";
            Connection = new NpgsqlConnection(connString);
            Connection.Open();
        }

        private void Close()
        {
            Connection.Close();
            Connection.Dispose();
            Connection = null;
        }

        private void Reconnect()
        {
            if (Connection != null)
            {
                Close();
            }
            
            Connect();
        }
    }
}
