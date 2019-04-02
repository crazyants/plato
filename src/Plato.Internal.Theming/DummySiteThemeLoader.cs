using System.Collections.Generic;
using Plato.Internal.Theming.Abstractions;
using Plato.Internal.Theming.Abstractions.Models;

namespace Plato.Internal.Theming
{
    public class DummySiteThemeLoader : ISiteThemeLoader
    {

        private readonly IThemeLoader _themeLoader;

        public DummySiteThemeLoader(IThemeLoader themeLoader)
        {
            _themeLoader = themeLoader;
        }

        public string RootPath => _themeLoader.RootPath;

        public IEnumerable<IThemeDescriptor> AvailableThemes => _themeLoader.AvailableThemes;

    }

}
