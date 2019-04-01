using System.Collections.Generic;

namespace Plato.Internal.Theming.Abstractions
{
    public interface IThemeFileManager
    {

        IEnumerable<IThemeFile> GetFiles(string themeId);

        IEnumerable<IThemeFile> GetFiles(string themeId, string relativePath);

        IThemeFile GetFile(string themeId, string relativePath);

    }
}
