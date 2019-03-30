using System;
using System.Linq;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Theming.Abstractions;
using Plato.Internal.Theming.Abstractions.Models;

namespace Plato.Theming.Services
{
    
    public class SiteThemeCreator : ISiteThemeCreator
    {
        
        public readonly ISiteThemeManager _siteThemeManager;
        private readonly IPlatoFileSystem _platoFileSystem;
        public readonly IThemeManager _themeManager;
        public readonly ISitesFolder _sitesFolder;

        public SiteThemeCreator(
            ISiteThemeManager siteThemeManager,
            IPlatoFileSystem platoFileSystem,
            IThemeManager themeManager,
            ISitesFolder sitesFolder)
        {
            _siteThemeManager = siteThemeManager;
            _platoFileSystem = platoFileSystem;
            _themeManager = themeManager;
            _sitesFolder = sitesFolder;
        }

        public ICommandResult<IThemeDescriptor> CreateTheme(string baseThemeId, string newThemeName)
        {

            // Create result
            var result = new CommandResult<ThemeDescriptor>();

            // Get base theme 
            IThemeDescriptor baseTheme = null;
            foreach (var theme in _themeManager.AvailableThemes)
            {
                if (theme.Id.Equals(baseThemeId, StringComparison.OrdinalIgnoreCase))
                {
                    baseTheme = theme;
                    break;
                }
            }

            // Ensure base theme exists
            if (baseTheme == null)
            {
                throw new Exception($"Could not locate the theme \"{baseThemeId}\".");
            }

            try
            {

                var newThemeId = newThemeName.ToSafeFileName();

                // Path to the new directory for our theme
                var targetPath = _platoFileSystem.Combine(
                    _siteThemeManager.RootPath, newThemeId);

                // Copy base theme to new directory
                _platoFileSystem.CopyDirectory(
                    baseTheme.FullPath,
                    targetPath,
                    true);

                // Update theme name 
                baseTheme.Name = newThemeName;
            
                // Update YAML 
                var update = _siteThemeManager.SaveDescriptor(newThemeId, baseTheme);
                if (!update.Succeeded)
                {
                    return result.Failed(update.Errors.ToArray());
                }

            }
            catch (Exception e)
            {
                return result.Failed(e.Message);
            }

            return result.Success(baseTheme);

        }

    }
}
