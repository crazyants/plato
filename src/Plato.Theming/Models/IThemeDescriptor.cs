using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Theming.Models
{
    public interface IThemeDescriptor
    {

        string Name { get; set; }

        string Description { get; set; }

        string Author { get; set; }
        
        string WebSite { get; set; }

        string Version { get; set; }

        string PlatoVersion { get; set; }

        string Category { get; set; }

    }
}
