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

        private readonly IThemeDescriptorUpdater _themeDescriptorUpdater;
        private readonly ISiteThemeManager _siteThemeManager;
        private readonly IPlatoFileSystem _platoFileSystem;
        public readonly IThemeManager _themeManager;
        public readonly ISitesFolder _sitesFolder;

        public SiteThemeCreator(
            ISiteThemeManager siteThemeManager,
            IPlatoFileSystem platoFileSystem,
            IThemeManager themeManager,
            ISitesFolder sitesFolder,
            IThemeDescriptorUpdater themeDescriptorUpdater)
        {
            _siteThemeManager = siteThemeManager;
            _platoFileSystem = platoFileSystem;
            _themeManager = themeManager;
            _sitesFolder = sitesFolder;
            _themeDescriptorUpdater = themeDescriptorUpdater;
        }

        public ICommandResult<IThemeDescriptor> CreateTheme(string baseThemeId, string newThemeName)
        {

            // Create result
            var result = new CommandResult<ThemeDescriptor>();

            // Get base theme 
            IThemeDescriptor baseThemeDescriptor = null;
            foreach (var descriptor in _themeManager.AvailableThemes)
            {
                if (descriptor.Id.Equals(baseThemeId, StringComparison.OrdinalIgnoreCase))
                {
                    baseThemeDescriptor = descriptor;
                    break;
                }
            }

            // Ensure base theme exists
            if (baseThemeDescriptor == null)
            {
                throw new Exception($"Could not locate the theme \"{baseThemeId}\".");
            }

            try
            {

                var newThemeId = newThemeName.ToSafeFileName();
                if (!string.IsNullOrEmpty(newThemeId))
                {
                    newThemeId = newThemeId.ToLower()
                        .Replace(" ", "-");
                }
                
                // Path to the new directory for our theme
                var targetPath = _platoFileSystem.Combine(
                    _siteThemeManager.RootPath, newThemeId);

                // Copy base theme to new directory within /sites/{SiteName/themes
                _platoFileSystem.CopyDirectory(
                    baseThemeDescriptor.FullPath,
                    targetPath,
                    true);

                // Update theme name 
                baseThemeDescriptor.Name = newThemeName;
            
                // Update YAML manifest
                var update = _themeDescriptorUpdater.UpdateThemeDescriptor(targetPath, baseThemeDescriptor);
                if (!update.Succeeded)
                {
                    return result.Failed(update.Errors.ToArray());
                }

            }
            catch (Exception e)
            {
                return result.Failed(e.Message);
            }

            return result.Success(baseThemeDescriptor);

        }

    }
}
