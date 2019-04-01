using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Theming.Abstractions;

namespace Plato.Internal.Theming
{

    public class ThemeFileManager : IThemeFileManager
    {
        private string _fullPathToCurrentTheme;

        private IThemeManager _themeManager;
        private readonly IPlatoFileSystem _fileSystem;

        public ThemeFileManager(IPlatoFileSystem filesystem)
        {
            _fileSystem = filesystem;
        }

        public void SetThemeManager(IThemeManager manager)
        {
            _themeManager = manager;
        }

        public IEnumerable<IThemeFile> GetFiles(string themeId)
        {

            // Ensure theme manager
            EnsureThemeManager();

            // Get available themes
            var availableThemes = _themeManager.AvailableThemes;
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
            _fullPathToCurrentTheme = _fileSystem.MapPath(theme.FullPath);

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
            var directory = _fileSystem.GetDirectoryInfo(theme.FullPath);
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

            // Ensure theme manager
            EnsureThemeManager();

            var file = GetFile(themeId, relativePath);
            if (file != null)
            {
                return file.Children.Reverse();
            }

            return new List<IThemeFile>();

        }

        public IThemeFile GetFile(string themeId, string relativePath)
        {

            // Ensure theme manager
            EnsureThemeManager();

            var files = GetFiles(themeId);
            if (files != null)
            {
                return GetThemeFileByRelativePathRecursively(relativePath, files);
            }

            return null;

        }

        public IEnumerable<IThemeFile> GetParents(string themeId, string relativePath)
        {

            // Ensure theme manager
            EnsureThemeManager();

            var file = GetFile(themeId, relativePath);
            if (file != null)
            {
                return RecurseParents(file).Reverse();
            }

            return null;

        }

        // ---------------


        void EnsureThemeManager()
        {
            if (_themeManager == null)
            {
                throw new Exception("A current theme manager has not been initialized. You must call the SetThemeManager method passing in a valid theme manager instance.");
            }
        }

        IEnumerable<IThemeFile> BuildDirectoriesRecursively(string path, IThemeFile parent = null,
            IList<IThemeFile> output = null)
        {

            if (output == null)
            {
                output = new List<IThemeFile>();
            }

            // Recurse directories
            var themeDirectory = _fileSystem.GetDirectoryInfo(path);

            // Add directory
            var root = new ThemeFile
            {
                Name = themeDirectory.Name,
                FullName = themeDirectory.FullName,
                RelativePath = themeDirectory.FullName.Replace(_fullPathToCurrentTheme, ""),
                Parent = parent
            };
            
            foreach (var directory in themeDirectory.GetDirectories())
            {
                // Add directory
                var themeFile = new ThemeFile
                {
                    Name = directory.Name,
                    FullName = directory.FullName,
                    RelativePath = directory.FullName.Replace(_fullPathToCurrentTheme, ""),
                    Parent = parent ?? root
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
