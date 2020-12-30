using System.Collections.Generic;
using System.Linq;
using MTCG.Domain;
using MTCG.Entities;

namespace MTCG.Mappers
{
    public static class PackageEntityMapper
    {
        public static List<Package> Map(IEnumerable<PackageEntity> entities)
        {
            return entities.Select(Map).ToList();
        }
        
        public static Package Map(PackageEntity obj)
        {
            if (obj is null)
            {
                return null;
            }
            
            var packageCreation = Package.Create(obj.Id);
            return packageCreation.Value;
        }
    }
}