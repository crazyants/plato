using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Theming.Abstractions;

namespace Plato.Internal.Theming
{
    public class ThemeFileManager : IThemeFileManager
    {

        public IEnumerable<IThemeFile> GetFiles(string themeId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IThemeFile> GetFiles(string themeId, string relativePath)
        {
            throw new NotImplementedException();
        }

        public IThemeFile GetFile(string themeId, string relativePath)
        {
            throw new NotImplementedException();
        }
    }
}
