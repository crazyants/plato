using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Plato.Theming.Locator;
using Plato.Theming.Models;

namespace Plato.Theming
{
    public class ThemeManager : IThemeManager
    {

        #region "Private Variables"

        readonly IThemeLocator _themeLocator;
        IEnumerable<IThemeDescriptor> _themeDescriptors;

        readonly string _contentRootPath;
        readonly string _virtualPathToModules;

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
            IThemeLocator moduleLocator)
        {
            _themeLocator = moduleLocator;
            _contentRootPath = hostingEnvironment.ContentRootPath;
            _virtualPathToModules = themeOptions.Value.VirtualPathToThemesFolder;
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
             new string[] { _contentRootPath + "\\" + _virtualPathToModules },
             "Themes", "theme.txt", false);
        }

        #endregion

    }
}
