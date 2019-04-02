using System.Collections.Generic;
using Plato.Internal.Theming.Abstractions;
using Plato.Internal.Theming.Abstractions.Models;

namespace Plato.Internal.Theming
{
    public class DummySiteThemeManager : ISiteThemeManager
    {

        private readonly IThemeManager _themeManager;

        public DummySiteThemeManager(IThemeManager themeManager)
        {
            _themeManager = themeManager;
        }

        public string RootPath => _themeManager.RootPath;

        public IEnumerable<IThemeDescriptor> AvailableThemes => _themeManager.AvailableThemes;

    }

}
