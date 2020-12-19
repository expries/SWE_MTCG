using System;
using System.Collections.Generic;
using MTCG.Resource;

namespace MTCG.Repository
{
    public interface IPackageRepository
    {
        public List<Package> GetAllPackages();

        public Package GetPackage(Guid packageId);
        
        public Package CreatePackage(Package package);

        public void DeletePackage(Guid packageId);
    }
}