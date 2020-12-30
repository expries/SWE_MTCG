using System;
using System.Collections.Generic;
using MTCG.Domain;

namespace MTCG.Repositories
{
    public interface IPackageRepository
    {
        public List<Package> GetAllPackages();

        public Package GetPackage(Guid packageId);
        
        public Package CreatePackage(Package package);

        public void DeletePackage(Package package);
    }
}