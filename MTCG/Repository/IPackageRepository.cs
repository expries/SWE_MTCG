using System;
using System.Collections.Generic;
using MTCG.Resource;

namespace MTCG.Repository
{
    public interface IPackageRepository
    {
        public List<Package> GetAllPackages();

        public Package GetPackage(Guid id);

        public Package GetRandomPackage();

        public Package CreatePackage(Package package);

        public bool DeletePackage(Guid id);
        
        public bool UpdatePackage(Package package);
    }
}