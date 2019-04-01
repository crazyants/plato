using System.Collections.Generic;

namespace Plato.Internal.Theming.Abstractions
{
    public interface IThemeFileManager
    {

        void SetThemeManager(IThemeManager manager);

        IEnumerable<IThemeFile> GetFiles(string themeId);

        IEnumerable<IThemeFile> GetFiles(string themeId, string relativePath);

        IThemeFile GetFile(string themeId, string relativePath);

        IEnumerable<IThemeFile> GetParents(string themeId, string relativePath);

    }
}
