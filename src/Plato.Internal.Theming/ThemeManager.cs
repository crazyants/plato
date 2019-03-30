using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Plato.Internal.Theming.Abstractions;
using Plato.Internal.Theming.Abstractions.Locator;
using Plato.Internal.Theming.Abstractions.Models;

namespace Plato.Internal.Theming
{
    public class ThemeManager : IThemeManager
    {

        #region "Private Variables"

        readonly IThemeLocator _themeLocator;
        IEnumerable<IThemeDescriptor> _themeDescriptors;

        readonly string _contentRootPath;
        readonly string _virtualPathToThemesFolder;

        #endregion

        #region "Public ReadOnly Propertoes"

        public IEnumerable<IThemeDescriptor> AvailableThemes
        {
            get
            {
                InitializeThemes();
                return _themeDescriptors;
            }
        }

        #endregion

        #region "Constructor"

        public ThemeManager(
            IHostingEnvironment hostingEnvironment,
            IOptions<ThemeOptions> themeOptions,
            IThemeLocator themeLocator)
        {
            _themeLocator = themeLocator;
            _contentRootPath = hostingEnvironment.ContentRootPath;
            _virtualPathToThemesFolder = themeOptions.Value.VirtualPathToThemesFolder;
            InitializeThemes();
        }

        #endregion

        #region "Private Methods"

        void InitializeThemes()
        {
            if (_themeDescriptors == null)
            {
                _themeDescriptors = new List<IThemeDescriptor>();
                LoadThemeDescriptors();
            }
        }

        void LoadThemeDescriptors()
        {
            _themeDescriptors = _themeLocator.LocateThemes(
                new string[] {_contentRootPath + "\\" + _virtualPathToThemesFolder},
                "Themes", "theme.txt", false);
        }

        #endregion

    }
}
