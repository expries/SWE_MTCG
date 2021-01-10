using System;

namespace MTCG.Database
{
    /// <summary>
    /// Marks which database column corresponds to a property
    /// </summary>
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
    }
}