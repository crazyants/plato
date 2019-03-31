using System.Collections.Generic;

namespace Plato.Internal.Theming.Abstractions.Models
{
    public interface IThemeDescriptor
    {
        
        string Id { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        string Author { get; set; }
        
        string WebSite { get; set; }

        string Version { get; set; }

        string PlatoVersion { get; set; }

        string Category { get; set; }

        string FullPath { get; set; }
        
        string Tags { get; set; }

        string this[string key] { get; }

        IEnumerable<string> Keys { get; }

        IDictionary<string, string> Configuration { get; }

    }

}
