using System;
using System.Collections.Generic;
using MTCG.Resources;

namespace MTCG.Repositories
{
    public interface IPackageRepository
    {
        public List<Package> GetAllPackages();

        public Package GetPackage(Guid id);

        public Package GetRandomPackage();

        public Package CreatePackage(List<Guid> cardIds);

        public bool DeletePackage(Guid id);
        
        public bool UpdatePackage(Guid id, List<Guid> cardIds);
    }
}