using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Theming.Abstractions;
using Plato.Internal.Theming.Abstractions.Locator;
using Plato.Internal.Theming.Abstractions.Models;

namespace Plato.Theming.Services
{

    public class SiteThemeManager : ISiteThemeManager
    {

        private readonly IThemeLocator _themeLocator;
        private IEnumerable<IThemeDescriptor> _themeDescriptors;

        public SiteThemeManager(
            IShellSettings shellSettings,
            IOptions<ThemeOptions> themeOptions,
            IThemeLocator themeLocator,
            ISitesFolder sitesFolder)
        {

            _themeLocator = themeLocator;
            
            RootPath = sitesFolder.Combine(
                sitesFolder.RootPath,
                shellSettings.Location,
                themeOptions.Value.VirtualPathToThemesFolder?.ToLower()); ;
            
            InitializeThemes();
        }
        
        #region "Implementation"

        public string RootPath { get; private set; }

        public IEnumerable<IThemeDescriptor> AvailableThemes
        {
            get
            {
                InitializeThemes();
                return _themeDescriptors;
            }
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
                new string[] { RootPath },
                "Themes",
                "theme.txt",
                false);
        }

        #endregion

    }

}
