using System;
using System.Data.Common;

namespace Plato.Internal.Models.Annotations
{
    public class ColumnNameAttribute : Attribute
    {
        public ColumnNameAttribute(string name)
        {
            Name = name;
        }

        public ColumnNameAttribute(string name, Type type)
        {
            Name = name;
            Type = type;
        }
        public ColumnNameAttribute(string name, Type type, short length)
        {
            Name = name;
            Type = type;
            Length = length;
        }
        
        public string Name { get; set; }

        public Type Type { get; set; }

        public short Length { get; set; }


    }
}
