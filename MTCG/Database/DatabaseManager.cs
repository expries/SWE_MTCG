using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Npgsql;
using Npgsql.Schema;

namespace MTCG.Database
{
    public class DatabaseManager
    {
        private NpgsqlConnection Connection { get; }
        
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
            
            string connString  = $"Host={Host};Username={Username};Password={Password};Database={Database}";
            Connection = new NpgsqlConnection(connString);
            Connection.Open();
        }

        public static void MapEnum<TEnum>(string name) where TEnum : struct, Enum
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<TEnum>(name);
        }

        public int Execute(string sql)
        {
            var cmd = new NpgsqlCommand(sql);
            return Execute(cmd);
        }

        public int Execute(string sql, object dataObject)
        {
            var cmd = GetCommand(sql, dataObject);
            return Execute(cmd);
        }
        
        public int Execute(NpgsqlCommand cmd)
        {
            cmd.Connection = Connection;
            return cmd.ExecuteNonQuery();
        }
        
        public TEntity QueryFirstOrDefault<TEntity>(string query) 
            where TEntity : class, new()
        {
            var cmd = new NpgsqlCommand(query);
            return QueryFirstOrDefault<TEntity>(cmd);
        }

        public TEntity QueryFirstOrDefault<TEntity>(string query, object dataObject) 
            where TEntity : class, new()
        {
            var cmd = GetCommand(query, dataObject);
            return QueryFirstOrDefault<TEntity>(cmd);
        }

        public TEntity QueryFirstOrDefault<TEntity>(NpgsqlCommand cmd) 
            where TEntity : class, new()
        {
            lock (Connection)
            {
                OpenReader(cmd);
                var record = GetNextRecord<TEntity>();
                CloseReader();
                return record;
            }
        }
        
        public List<TEntity> Query<TEntity>(string query, int limit = 100) 
            where TEntity : class, new()
        {
            var cmd = new NpgsqlCommand(query);
            return Query<TEntity>(cmd, limit);
        }

        public List<TEntity> Query<TEntity>(string query, object dataObject, int limit = 100) 
            where TEntity : class, new()
        {
            var cmd = GetCommand(query, dataObject);
            return Query<TEntity>(cmd, limit);
        }
        
        public List<TEntity> Query<TEntity>(NpgsqlCommand cmd, int limit = 100) 
            where TEntity : class, new()
        {
            lock (Connection)
            {
                OpenReader(cmd);
                var records = GetRecords<TEntity>(limit);
                CloseReader();
                return records;
            }
        }

        private void OpenReader(NpgsqlCommand cmd)
        {
            cmd.Connection = Connection;
            _reader = cmd.ExecuteReader();
        }

        private void CloseReader()
        {
            _reader.Close();
        }

        private static NpgsqlCommand GetCommand(string sql, object dataObject)
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
            int count = 0;
            TEntity record;

            while ((record = GetNextRecord<TEntity>()) != null && count < limit)
            {
                records.Add(record);
                count++;
            }

            return records;
        }
        
        private TEntity GetNextRecord<TEntity>() where TEntity : class, new()
        {
            _reader.Read();
            return GetRecord<TEntity>();
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
                AddColumnToRecord(column, record);
            }

            return record;
        }

        private void AddColumnToRecord(NpgsqlDbColumn column, IDictionary<string, object> record)
        {
            if (column.ColumnOrdinal is null)
            {
                return;
            }
                
            int ordinal = (int) column.ColumnOrdinal;
            var value = _reader.GetValue(ordinal);
            record.Add(column.ColumnName, value);
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
    }
}
