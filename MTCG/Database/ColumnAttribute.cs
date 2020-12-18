using System;

namespace MTCG.Database
{
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
    }
}