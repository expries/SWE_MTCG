using MTCG.Database.Entity;
using MTCG.Resource;

namespace MTCG.Mapper
{
    public class PackageEntityMapper : Mapper<PackageEntity, Package>
    {
        public override Package Map(PackageEntity obj)
        {
            return obj is null ? null : new Package(obj.PackageId);
        }
    }
}