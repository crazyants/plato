using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Theming.Abstractions;

namespace Plato.Theming.Services
{
    
    public class SiteThemeFileManager : ISiteThemeFileManager
    {

        private string _fullPathToCurrentTheme;
        
        private readonly ISiteThemeManager _siteThemeManager;
        private readonly IPlatoFileSystem _platoFileSystem;
    
        public SiteThemeFileManager(

            IOptions<ThemeOptions> themeOptions,
            IPlatoFileSystem platoFilesystem,
            IShellSettings shellSettings,
            ISitesFolder sitesFolder,
            ISiteThemeManager siteThemeManager)
        {

            _platoFileSystem = platoFilesystem;
            _siteThemeManager = siteThemeManager;
            
        }

        public IEnumerable<IThemeFile> GetFiles(string themeId)
        {

            // Get theme to list
            var theme = _siteThemeManager.AvailableThemes
                .FirstOrDefault(t => t.Id.Equals(themeId, StringComparison.OrdinalIgnoreCase));

            // Ensure we found the theme
            if (theme == null)
            {
                throw new Exception($"A theme folder named \"{themeId}\" could not be found!");
            }

            // Full path to theme we are listing files for
            _fullPathToCurrentTheme = _platoFileSystem.MapPath(theme.FullPath);

            // Iterate files
            var themeFiles = new List<IThemeFile>();

            // Add directories
            var directories = GetDirectoriesRecursively(theme.FullPath);
            if (directories != null)
            {
                themeFiles.AddRange(directories);
            }

            // Add files
            var directory = _platoFileSystem.GetDirectoryInfo(theme.FullPath);
            foreach (var file in directory.GetFiles())
            {
                themeFiles.Add(new ThemeFile()
                {
                    Name = file.Name,
                    FullName = file.FullName,
                    RelativePath = file.FullName.Replace(_fullPathToCurrentTheme, ""),
                });
            }

            return themeFiles;
            
        }

        IEnumerable<IThemeFile> GetDirectoriesRecursively(
            string themePath,
            IThemeFile parentDirectory = null,
            IList<IThemeFile> output = null)
        {

            if (output == null)
            {
                output = new List<IThemeFile>();
            }

            // Recurse directories
            var themeDirectory = _platoFileSystem.GetDirectoryInfo(themePath);
            foreach (var directory in themeDirectory.GetDirectories())
            {
                // Add directory
                var themeFile = new ThemeFile
                {
                    Name = directory.Name,
                    FullName = directory.FullName,
                    RelativePath = directory.FullName.Replace(_fullPathToCurrentTheme, "")
                };

                // Add files from directory
                foreach (var file in directory.GetFiles())
                {
                    themeFile.Children.Add(new ThemeFile()
                    {
                        Name = file.Name,
                        FullName = file.FullName,
                        RelativePath = file.FullName.Replace(_fullPathToCurrentTheme, ""),
                        Parent = themeFile
                    });
                }

                if (parentDirectory != null)
                {
                    parentDirectory.Children.Add(themeFile);
                }
                else
                {
                    output.Add(themeFile);
                }

                GetDirectoriesRecursively(directory.FullName, themeFile, output);

            }

            return output;

        }
        
    }

}
