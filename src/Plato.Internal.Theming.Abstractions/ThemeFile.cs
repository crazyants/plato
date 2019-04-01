using System.Collections.Generic;

namespace Plato.Internal.Theming.Abstractions
{

    public interface IThemeFile
    {
        string Name { get; set; }

        string FullName { get; set; }

        string RelativePath { get; set; }

        bool IsDirectory { get; set; }

        IList<IThemeFile> Children { get; set; }

        IThemeFile Parent { get; set; }

    }

    public class ThemeFile : IThemeFile
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public string RelativePath { get; set; }

        public bool IsDirectory { get; set; }

        public IList<IThemeFile> Children { get; set; } = new List<IThemeFile>();
        
        public IThemeFile Parent { get; set; }

    }

}
