using System.Collections.Generic;
using Plato.Internal.Theming.Abstractions;

namespace Plato.Theming.Services
{
    public interface ISiteThemeFileManager : IThemeFileManager
    {

        IEnumerable<IThemeFile> GetParents(string themeId, string relativePath);
    }

}
