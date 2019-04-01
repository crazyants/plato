using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Theming.Abstractions;
using Plato.Internal.Theming.Abstractions.Locator;
using Plato.Internal.Theming.Abstractions.Models;
using Plato.Internal.Yaml;

namespace Plato.Theming.Services
{

    public class SiteThemeManager : ISiteThemeManager
    {
        
        private IEnumerable<IThemeDescriptor> _themeDescriptors;
        private readonly IPlatoFileSystem _platoFileSystem;
        private readonly IThemeLocator _themeLocator;
        
     
        public SiteThemeManager(
            IOptions<ThemeOptions> themeOptions,
            IPlatoFileSystem platoFilesystem,
            IShellSettings shellSettings,
            IThemeLocator themeLocator,
            ISitesFolder sitesFolder)
        {

            _platoFileSystem = platoFilesystem;
            _themeLocator = themeLocator;

            RootPath = platoFilesystem.Combine(
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
