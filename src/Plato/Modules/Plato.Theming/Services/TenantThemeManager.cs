using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Theming.Abstractions;
using Plato.Internal.Theming.Abstractions.Locator;
using Plato.Internal.Theming.Abstractions.Models;

namespace Plato.Theming.Services
{
    public class TenantThemeManager : ITenantThemeManager
    {

        private readonly IThemeLocator _themeLocator;
        private readonly IUploadFolder _uploadFolder;

        private IEnumerable<IThemeDescriptor> _themeDescriptors;
        private readonly string _contentRootPath;
        private readonly string _virtualPathToThemesFolder;

        public TenantThemeManager(
            IOptions<ThemeOptions> themeOptions,
            IThemeLocator themeLocator,
            IUploadFolder uploadFolder)
        {
            _themeLocator = themeLocator;
            _uploadFolder = uploadFolder;
            _contentRootPath = uploadFolder.Path;
            _virtualPathToThemesFolder = themeOptions.Value.VirtualPathToThemesFolder;
            InitializeThemes();
        }


        #region "Implementation"

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
                new string[] { _contentRootPath + "\\" + _virtualPathToThemesFolder },
                "Themes", "theme.txt", false);
        }

        #endregion

    }
}
