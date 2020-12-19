using System;

namespace MTCG.Database.Entity
{
    public class PackageEntity
    {
        [Column(Name="packageID")]
        public Guid PackageId { get; set; }
        
        [Column(Name="price")]
        public double Price { get; set; }
    }
}