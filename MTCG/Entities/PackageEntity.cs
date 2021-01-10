using System;
using MTCG.Database;

namespace MTCG.Entities
{
    public class PackageEntity
    {
        [Column(Name="packageID")]
        public Guid Id { get; set; }
        
        [Column(Name="price")]
        public double Price { get; set; }
    }
}