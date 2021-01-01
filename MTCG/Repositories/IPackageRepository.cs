using System;
using System.Collections.Generic;
using MTCG.Domain;

namespace MTCG.Repositories
{
    public interface IPackageRepository
    {
        public List<Package> GetAll();

        public Package Get(Guid packageId);
        
        public Package Create(Package package);

        public void Delete(Package package);
    }
}