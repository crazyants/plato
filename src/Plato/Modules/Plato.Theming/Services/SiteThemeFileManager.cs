using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Theming.Abstractions;

namespace Plato.Theming.Services
{
    
    public class SiteThemeFileManager : ISiteThemeFileManager
    {

        private string _fullPathToCurrentTheme;
        
        private readonly ISiteThemeManager _siteThemeManager;
        private readonly IPlatoFileSystem _platoFileSystem;
    
        public SiteThemeFileManager(
            IPlatoFileSystem platoFilesystem,
            ISiteThemeManager siteThemeManager)
        {
            _platoFileSystem = platoFilesystem;
            _siteThemeManager = siteThemeManager;
        }

        public IEnumerable<IThemeFile> GetFiles(string themeId)
        {

            // Get available themes
            var availableThemes = _siteThemeManager.AvailableThemes;
            if (availableThemes == null)
            {
                throw new Exception($"Could not list files for theme \"{themeId}\" as no themes exist!");
            }

            // Get theme to list
            var theme = availableThemes.FirstOrDefault(t => t.Id.Equals(themeId, StringComparison.OrdinalIgnoreCase));

            // Ensure we found the theme
            if (theme == null)
            {
                throw new Exception($"A theme folder named \"{themeId}\" could not be found!");
            }

            // Full path to theme we are listing files for
            _fullPathToCurrentTheme = _platoFileSystem.MapPath(theme.FullPath);

            // Build output
            var output = new List<IThemeFile>();

            // Add directories
            var directories = BuildDirectoriesRecursively(theme.FullPath);
            if (directories != null)
            {
                foreach (var childDirectory in directories)
                {
                    output.Add(childDirectory);
                }
            }

            // Add files
            var directory = _platoFileSystem.GetDirectoryInfo(theme.FullPath);
            foreach (var file in directory.GetFiles())
            {
                output.Add(new ThemeFile()
                {
                    Name = file.Name,
                    FullName = file.FullName,
                    RelativePath = file.FullName.Replace(_fullPathToCurrentTheme, ""),
                });
            }

            return output;
            
        }

        public IEnumerable<IThemeFile> GetFiles(string themeId, string relativePath)
        {
            var file = GetFile(themeId, relativePath);
            if (file != null)
            {
                return file.Children.Reverse();
            }

            return new List<IThemeFile>();

        }

        public IThemeFile GetFile(string themeId, string relativePath)
        {
            var files = GetFiles(themeId);
            if (files != null)
            {
                return GetThemeFileByRelativePathRecursively(relativePath, files);
            }

            return null;

        }

        public IEnumerable<IThemeFile> GetParents(string themeId, string relativePath)
        {
            var file = GetFile(themeId, relativePath);
            if (file != null)
            {
                return RecurseParents(file).Reverse();
            }

            return null;

        }
        
        // ---------------

        IEnumerable<IThemeFile> BuildDirectoriesRecursively(string path, IThemeFile parent = null, IList<IThemeFile> output = null)
        {

            if (output == null)
            {
                output = new List<IThemeFile>();
            }

            // Recurse directories
            var themeDirectory = _platoFileSystem.GetDirectoryInfo(path);
            foreach (var directory in themeDirectory.GetDirectories())
            {
                // Add directory
                var themeFile = new ThemeFile
                {
                    Name = directory.Name,
                    FullName = directory.FullName,
                    RelativePath = directory.FullName.Replace(_fullPathToCurrentTheme, ""),
                    Parent = parent
                };

                // Add files to directory
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

                if (parent != null)
                {
                    parent.Children.Add(themeFile);
                }
                else
                {
                    output.Add(themeFile);
                }

                BuildDirectoriesRecursively(directory.FullName, themeFile, output);

            }

            return output;

        }

        IThemeFile GetThemeFileByRelativePathRecursively(string relativePath, IEnumerable<IThemeFile> themeFiles)
        {
            
            foreach (var themeFile in themeFiles)
            {
                if (themeFile.RelativePath.Equals(relativePath, StringComparison.OrdinalIgnoreCase))
                {
                    return themeFile;
                }

                if (themeFile.Children.Any())
                {
                    return GetThemeFileByRelativePathRecursively(relativePath, themeFile.Children);
                }
                
            }

            return null;

        }

        IEnumerable<IThemeFile> RecurseParents(IThemeFile themeFile, IList<IThemeFile> output = null)
        {

            if (output == null)
            {
                output = new List<IThemeFile>();
            }
            
            if (themeFile.Parent != null)
            {
                output.Add(themeFile);
                RecurseParents(themeFile.Parent, output);
            }
            else
            {
                output.Add(themeFile);
            }
            
            return output;

        }
        
    }

}
