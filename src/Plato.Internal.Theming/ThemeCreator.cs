using System;
using System.Linq;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Theming.Abstractions;
using Plato.Internal.Theming.Abstractions.Models;

namespace Plato.Internal.Theming
{
    
    public class ThemeCreator : IThemeCreator
    {

        private readonly IThemeUpdater _themeUpdater;
        private readonly ISiteThemeLoader _siteThemeLoader;
        private readonly IPlatoFileSystem _platoFileSystem;
        public readonly IThemeLoader _themeLoader;
     
        public ThemeCreator(
            ISiteThemeLoader siteThemeLoader,
            IPlatoFileSystem platoFileSystem,
            IThemeLoader themeLoader,
            IThemeUpdater themeUpdater)
        {
            _siteThemeLoader = siteThemeLoader;
            _platoFileSystem = platoFileSystem;
            _themeLoader = themeLoader;
            _themeUpdater = themeUpdater;
        }

        public ICommandResult<IThemeDescriptor> CreateTheme(string sourceThemeId, string newThemeName)
        {

            // Create result
            var result = new CommandResult<ThemeDescriptor>();

            // Get base theme 
            var baseDescriptor =
                _themeLoader.AvailableThemes.FirstOrDefault(t =>
                    t.Id.Equals(sourceThemeId, StringComparison.OrdinalIgnoreCase));

            // Ensure base theme exists
            if (baseDescriptor == null)
            {
                throw new Exception($"Could not locate the base theme \"{sourceThemeId}\".");
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
                    _siteThemeLoader.RootPath, newThemeId);

                // Copy base theme to new directory within /sites/{SiteName/themes
                _platoFileSystem.CopyDirectory(
                    baseDescriptor.FullPath,
                    targetPath,
                    true);

                // Update theme name 
                baseDescriptor.Name = newThemeName;
                baseDescriptor.FullPath = targetPath;

                // Update YAML manifest
                var update = _themeUpdater.UpdateTheme(targetPath, baseDescriptor);
                if (!update.Succeeded)
                {
                    return result.Failed(update.Errors.ToArray());
                }

            }
            catch (Exception e)
            {
                return result.Failed(e.Message);
            }

            return result.Success(baseDescriptor);

        }

    }
}
