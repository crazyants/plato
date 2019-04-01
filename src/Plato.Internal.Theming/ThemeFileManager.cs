using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Theming.Abstractions;

namespace Plato.Internal.Theming
{

    public class ThemeFileManager : IThemeFileManager
    {
    
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
                throw new Exception($"Could not list files for theme \"{themeId}\". No themes could be located!");
            }

            // Get theme to list
            var theme = availableThemes.FirstOrDefault(t => t.Id.Equals(themeId, StringComparison.OrdinalIgnoreCase));
            if (theme == null)
            {
                throw new Exception($"A theme folder named \"{themeId}\" could not be found!");
            }
            
            // Build output
            var output = new List<IThemeFile>();
            
            // Recurse directories
            var themeDirectory = _fileSystem.GetDirectoryInfo(theme.FullPath);

            // Add our theme folder as the root
            var themeRoot = new ThemeFile
            {
                Name = themeDirectory.Name,
                FullName = themeDirectory.FullName,
                IsDirectory = true
            };
            
            // Add child directories to our root
            var directories = BuildDirectoriesRecursively(theme.FullPath, themeRoot);
            if (directories != null)
            {
                foreach (var childDirectory in directories)
                {
                    output.Add(childDirectory);
                }
            }

            // Add files
            foreach (var file in themeDirectory.GetFiles())
            {
                output.Add(new ThemeFile()
                {
                    Name = file.Name,
                    FullName = file.FullName,
                    Parent = themeRoot
                });
            }
            
            // We need an explicit type to pass by reference
            var list = ((IList<IThemeFile>) output.ToList());

            // Update list with relative paths
            BuildRelativePathsRecursively(ref list, _fileSystem.MapPath(theme.FullPath));

            // Update updated list
            return list;
            
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

        public async Task<string> ReadFileAsync(IThemeFile themeFile)
        {

            if (themeFile == null)
            {
                return string.Empty;
            }

            if (themeFile.IsDirectory)
            {
                return string.Empty;
            }

            return await _fileSystem.ReadFileAsync(themeFile.FullName);
            
        }

        public async Task SaveFileAsync(IThemeFile themeFile, string contents)
        {

            if (themeFile == null)
            {
                throw new ArgumentNullException(nameof(themeFile));
            }

            if (themeFile.IsDirectory)
            {
                return;
            }

            await _fileSystem.SaveFileAsync(themeFile.FullName, contents);
        }

        // ---------------
        
        void EnsureThemeManager()
        {
            if (_themeManager == null)
            {
                throw new Exception("A theme manager has not been initialized. You must call the SetThemeManager method passing in a valid IThemeManager implementation.");
            }
        }

        IEnumerable<IThemeFile> BuildDirectoriesRecursively(string path, IThemeFile parent = null)
        {
            
            // Recurse directories
            var themeDirectory = _fileSystem.GetDirectoryInfo(path);
            foreach (var directory in themeDirectory.GetDirectories())
            {
                // Add directory
                var themeFile = new ThemeFile
                {
                    Name = directory.Name,
                    FullName = directory.FullName,
                    Parent = parent,
                    IsDirectory = true
                };

                // Add files to directory
                foreach (var file in directory.GetFiles())
                {
                    themeFile.Children.Add(new ThemeFile()
                    {
                        Name = file.Name,
                        FullName = file.FullName,
                        Parent = themeFile
                    });
                }
                
                // If a parent exists add current file as a child of the parent
                parent?.Children.Add(themeFile);
                
                // Recurse until we've processed all directories 
                BuildDirectoriesRecursively(directory.FullName, themeFile);
              
            }

            return parent?.Children ?? null;

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
                    // Continue searching until we find results
                    var files = GetThemeFileByRelativePathRecursively(relativePath, themeFile.Children);
                    if (files != null)
                    {
                        return files;
                    }
                }

            }

            return null;

        }
        
         void BuildRelativePathsRecursively(ref IList<IThemeFile> themeFiles, string rootPath)
        {

            foreach (var themeFile in themeFiles)
            {

                themeFile.RelativePath = themeFile.FullName.Replace(rootPath, "");
                
                if (themeFile.Children.Any())
                {
                    var children = themeFile.Children;
                    BuildRelativePathsRecursively(ref children, rootPath);
                }

            }
            
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
