using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions;
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

        private const string ByThemeFileNameFormat = "Theme.{0}";

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

        public IEnumerable<ThemeFile> ListFiles(string themeId)
        {

            // Get theme to list
            var theme = AvailableThemes.FirstOrDefault(t => t.Id.Equals(themeId, StringComparison.OrdinalIgnoreCase));
            if (theme == null)
            {
                throw new Exception($"A theme folder named {themeId} could not be found!");
            }
            
            return ListFilesInternal(theme.FullPath);

        }

        private IEnumerable<ThemeFile> ListFilesInternal(string path)
        {

            var output = new List<ThemeFile>();

            // Process directories
            var directories = _platoFileSystem.ListDirectories(path);
            foreach (var directory in directories)
            {

                var themeFile = new ThemeFile
                {
                    Name = directory.Name
                };

                foreach (var file in directory.GetFiles())
                {
                    themeFile.Children.Add(new ThemeFile()
                    {
                        Name = file.Name
                    });
                }

                output.Add(themeFile);

            }
            
            // Process files
            var currentDirectory = _platoFileSystem.GetDirectoryInfo(path);
            foreach (var file in currentDirectory.GetFiles())
            {
                output.Add(new ThemeFile()
                {
                    Name = file.Name
                });
            }
            
            return output;

        }
        
        public ICommandResult<IThemeDescriptor> UpdateThemeDescriptor(string themeId, IThemeDescriptor descriptor)
        {
            
            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (string.IsNullOrEmpty(descriptor.Name))
            {
                throw new ArgumentNullException(nameof(descriptor.Name));
            }
            
            // Path to theme manifest file
            var fileName = string.Format(ByThemeFileNameFormat, "txt");
            var manifestPath = _platoFileSystem.MapPath(
                _platoFileSystem.Combine(RootPath, themeId, fileName));

            // Configure YAML configuration
            var configurationProvider = new YamlConfigurationProvider(new YamlConfigurationSource
            {
                Path = manifestPath,
                Optional = false
            });

            // Build configuration
            foreach (var key in descriptor.Keys)
            {
                if (!string.IsNullOrEmpty(descriptor[key]))
                {
                    configurationProvider.Set(key, descriptor[key]);
                }
            }

            var result = new CommandResult<ThemeDescriptor>();

            try
            {
                configurationProvider.Commit();
            }
            catch (Exception e)
            {
                return result.Failed(e.Message);
            }

            return result.Success(descriptor);
            
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
