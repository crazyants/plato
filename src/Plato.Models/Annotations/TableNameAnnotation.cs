using System;

namespace Plato.Models.Annotations
{
    public class TableNameAttribute : Attribute
    {
        public TableNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
