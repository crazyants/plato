using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Abstractions.Layout
{
 
    [AttributeUsage(AttributeTargets.Class)]
    public class PlatoViewComponent : Attribute
    {
        public PlatoViewComponent(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

    }
}
