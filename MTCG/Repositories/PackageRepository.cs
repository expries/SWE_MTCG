using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Resources;

namespace MTCG.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        private readonly Dictionary<Guid, Package> _packages;

        public PackageRepository()
        {
            _packages = new Dictionary<Guid, Package>();
        }

        public Package CreatePackage(List<Guid> cardIds)
        {
            var package = new Package(cardIds) {Id = Guid.NewGuid()};
            _packages.Add(package.Id, package);
            return package;
        }

        public Package GetPackage(Guid id)
        {
            return _packages.ContainsKey(id) ? _packages[id] : null;
        }

        public bool DeletePackage(Guid id)
        {
            if (_packages.ContainsKey(id)) return false;
            _packages.Remove(id);
            return true;
        }

        public bool UpdatePackage(Guid id, List<Guid> cardIds)
        {
            if (!_packages.ContainsKey(id)) return false;
            _packages[id].SetCardIds(cardIds);
            return true;
        }

        public List<Package> GetAllPackages()
        {
            return _packages.Values.ToList();
        }

        public Package GetRandomPackage()
        {
            var packageList = _packages.Values.ToList();
            if (!packageList.Any()) return null;
            int randomIndex = new Random().Next(packageList.Count);
            return packageList[randomIndex];
        }
    }
}