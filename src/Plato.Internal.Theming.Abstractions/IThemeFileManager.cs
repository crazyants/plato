using System.Collections.Generic;

namespace Plato.Internal.Theming.Abstractions
{
    public interface IThemeFileManager
    {

        IEnumerable<IThemeFile> GetFiles(string themeId);

    }
}
