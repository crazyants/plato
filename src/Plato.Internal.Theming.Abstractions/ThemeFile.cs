using System.Collections.Generic;

namespace Plato.Internal.Theming.Abstractions
{
    public class ThemeFile
    {
        public string Name { get; set; }

        public IList<ThemeFile> Children { get; set; } = new List<ThemeFile>();
        
    }

}
