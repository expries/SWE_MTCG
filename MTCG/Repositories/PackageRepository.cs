using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Database;
using MTCG.Domain;
using MTCG.Domain.Cards;
using MTCG.Entities;
using MTCG.Mappers;

namespace MTCG.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        private readonly DatabaseManager _db;

        public PackageRepository(DatabaseManager db)
        {
            _db = db;
        }

        public void Delete(Package package)
        {
            const string sql = "DELETE FROM package WHERE packageID = @PackageId";
            _db.Execute(sql, new {PackageId = package.Id});
        }

        public Package Get(Guid packageId)
        {
            const string sql = "SELECT packageID, Price FROM package " +
                               "WHERE packageID = @PackageId";
            
            var entity = _db.QueryFirstOrDefault<PackageEntity>(sql, new {PackageId = packageId});
            var package = MapEntity(entity);
            return package;
        }

        public List<Package> GetAll()
        {
            const string sql = "SELECT packageID, price FROM package";
            var entities = _db.Query<PackageEntity>(sql);
            var packages = entities.Select(MapEntity).ToList();
            return packages;
        }
        
        public Package Create(Package package)
        {
            const string sql = "INSERT INTO package (packageID, price) " +
                               "VALUES (@PackageId, @Price)";

            _db.Execute(sql, new {PackageId = package.Id, Price = package.Price});
            return package;
        }
        
        private Package MapEntity(PackageEntity entity)
        {
            if (entity is null)
            {
                return null;
            }
            
            var package = Package.Create(entity.Id).Value;
            var cards = GetCards(package.Id);
            cards.ForEach(card => package.AddCard(card));
            return package;
        }

        private List<Card> GetCards(Guid packageId)
        {
            const string sql = "SELECT cardID, name, type, element, damage, fk_packageID, monsterType " + 
                               "FROM card WHERE fk_packageId = @PackageId";
            
            var entities = _db.Query<CardEntity>(sql, new {PackageId = packageId});
            var cards = CardEntityMapper.MapIgnoreErrors(entities);
            return cards;
        }
    }
}