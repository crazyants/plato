using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Theming.Abstractions
{
    public interface IThemeFileManager
    {

        void SetThemeLoader(IThemeLoader loader);

        IEnumerable<IThemeFile> GetFiles(string themeId);

        IEnumerable<IThemeFile> GetFiles(string themeId, string relativePath);

        IThemeFile GetFile(string themeId, string relativePath);

        IEnumerable<IThemeFile> GetParents(string themeId, string relativePath);

        Task<string> ReadFileAsync(IThemeFile themeFile);

        Task SaveFileAsync(IThemeFile themeFile, string contents);

    }

}
