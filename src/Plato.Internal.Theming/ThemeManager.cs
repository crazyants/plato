using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Theming.Abstractions;
using Plato.Internal.Theming.Abstractions.Locator;
using Plato.Internal.Theming.Abstractions.Models;
using Plato.Internal.Yaml;

namespace Plato.Internal.Theming
{
    public class ThemeManager : IThemeManager
    {
        private const string ByThemeFileNameFormat = "Theme.{0}";

        private IEnumerable<IThemeDescriptor> _themeDescriptors;

        private readonly IThemeLocator _themeLocator;
        private readonly IPlatoFileSystem _platoFileSystem;
        
        public ThemeManager(
            IHostingEnvironment hostingEnvironment,
            IOptions<ThemeOptions> themeOptions,
            IThemeLocator themeLocator,
            IPlatoFileSystem platoFileSystem)
        {
            _themeLocator = themeLocator;
            _platoFileSystem = platoFileSystem;

            var contentRootPath = hostingEnvironment.ContentRootPath;
            var virtualPathToThemesFolder = themeOptions.Value.VirtualPathToThemesFolder;

            RootPath = _platoFileSystem.Combine(
                contentRootPath,
                virtualPathToThemesFolder);

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
            //if (_themeDescriptors == null)
            //{
                _themeDescriptors = new List<IThemeDescriptor>();
                LoadThemeDescriptors();
            //}
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
