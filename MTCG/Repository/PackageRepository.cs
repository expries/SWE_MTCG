using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Database;
using MTCG.Database.Entity;
using MTCG.Mapper;
using MTCG.Resource;
using MTCG.Resource.Cards;

namespace MTCG.Repository
{
    public class PackageRepository : IPackageRepository
    {
        private readonly DatabaseManager _db;

        public PackageRepository(DatabaseManager db)
        {
            _db = db;
        }

        public void DeletePackage(Guid packageId)
        {
            const string sql = "DELETE FROM package WHERE packageid = @id";
            _db.ExecuteNonQuery(sql, new {id = packageId});
        }

        public Package CreatePackage(Package package)
        {
            const string sql = "INSERT INTO package (packageid, price) " +
                               "VALUES (@packageId, @price)";

            _db.ExecuteNonQuery(sql, new {packageId = package.Id, price = package.Price});
            return GetPackage(package.Id);
        }

        public Package GetPackage(Guid packageId)
        {
            const string sql = "SELECT packageid, price FROM package " +
                               "WHERE packageid = @id";
            
            var entity = _db.FetchFirstFromQuery<PackageEntity>(sql, new {id = packageId});
            var packageMapper = new PackageEntityMapper();
            var package = packageMapper.Map(entity);
            
            if (package is null)
            {
                return null;
            }
            
            var cards = GetCardsInPackage(package.Id);
            cards.ForEach(card => package.AddCard(card));
            return package;
        }

        public List<Package> GetAllPackages()
        {
            const string sql = "SELECT packageid, price FROM package";
            var entities = _db.FetchFromQuery<PackageEntity>(sql);
            var packageMapper = new PackageEntityMapper();
            var packages = packageMapper.Map(entities).ToList();

            foreach (var package in packages)
            {
                var cards = GetCardsInPackage(package.Id);
                cards.ForEach(card => package.AddCard(card));
            }
            
            return packages;
        }
        
        private List<Card> GetCardsInPackage(Guid packageId)
        {
            const string sql = "SELECT cardid, name, type, element, damage, fk_packageid, monstertype FROM card " +
                               "WHERE fk_packageId = @id";
            
            var entities = _db.FetchFromQuery<CardEntity>(sql, new {id = packageId});
            var cardMapper = new CardEntityMapper();
            var cards = cardMapper.Map(entities).ToList();
            return cards;
        }
    }
}