using System;
using System.Collections.Generic;
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

        public void DeletePackage(Package package)
        {
            const string sql = "DELETE FROM package WHERE packageid = @id";
            _db.ExecuteNonQuery(sql, new {id = package.Id});
        }

        public Package GetPackage(Guid packageId)
        {
            const string sql = "SELECT packageid, price FROM package " +
                               "WHERE packageid = @id";
            
            var entity = _db.QueryFirstOrDefault<PackageEntity>(sql, new {id = packageId});

            if (entity is null)
            {
                return null;
            }
            
            var package = PackageEntityMapper.Map(entity);
            var cards = GetCardsInPackage(package.Id);
            cards.ForEach(card => package.AddCard(card));
            return package;
        }

        public List<Package> GetAllPackages()
        {
            const string sql = "SELECT packageid, price FROM package";
            var entities = _db.Query<PackageEntity>(sql);
            var packages = PackageEntityMapper.Map(entities);

            foreach (var package in packages)
            {
                var cards = GetCardsInPackage(package.Id);
                cards.ForEach(card => package.AddCard(card));
            }
            
            return packages;
        }
        
        public Package CreatePackage(Package package)
        {
            const string sql = "INSERT INTO package (packageid, price) " +
                               "VALUES (@packageId, @price)";

            _db.ExecuteNonQuery(sql, new {packageId = package.Id, price = package.Price});
            return GetPackage(package.Id);
        }

        private List<Card> GetCardsInPackage(Guid packageId)
        {
            const string sql = "SELECT cardid, name, type, element, damage, fk_packageid, monstertype " + 
                               "FROM card_info WHERE fk_packageId = @id";
            
            var entities = _db.Query<CardEntity>(sql, new {id = packageId});
            var cards = CardEntityMapper.Map(entities);
            return cards;
        }
    }
}