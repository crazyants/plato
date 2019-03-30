using System;
using System.Collections.Generic;
using System.IO;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Theming.Abstractions.Locator;
using Plato.Internal.Theming.Abstractions.Models;

namespace Plato.Internal.Theming
{
    public class ThemeLocator : IThemeLocator
    {

        private const string NameSection = "name";
        private const string PathSection = "path";
        private const string DescriptionSection = "description";
        private const string VersionSection = "version";
        private const string PlatoVersionSection = "orchardversion";
        private const string AuthorSection = "author";
        private const string WebsiteSection = "website";
        private const string TagsSection = "tags";
       
        private readonly IPlatoFileSystem _fileSystem;
        
        public ThemeLocator(IPlatoFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        #region "Implementation"

        public IEnumerable<IThemeDescriptor> LocateThemes(
            IEnumerable<string> paths,
            string extensionType,
            string manifestName,
            bool manifestIsOptional)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));

            var descriptors = new List<IThemeDescriptor>();
            foreach (var path in paths)
            {
                descriptors.AddRange(
                    AvailableThemes(
                        path,
                        extensionType,
                        manifestName,
                        manifestIsOptional)
                );
            }

            return descriptors;

        }

        #endregion

        #region "Private Methods"

        private IEnumerable<IThemeDescriptor> AvailableThemes(
            string path, 
            string extensionType,
            string manifestName,
            bool manifestIsOptional)
        {
            return AvailableThemesInFolder(
                path,
                extensionType,
                manifestName,
                manifestIsOptional).ToReadOnlyCollection();

        }

        private IEnumerable<IThemeDescriptor> AvailableThemesInFolder(
            string path,
            string themeType,
            string manifestName,
            bool manifestIsOptional)
        {
            var localList = new List<IThemeDescriptor>();

            if (string.IsNullOrEmpty(path))
            {
                return localList;
            }

            var subfolders = _fileSystem.ListDirectories(path);
            foreach (var subfolder in subfolders)
            {

                var themeId = subfolder.Name;
                var manifestPath = _fileSystem.Combine(path, themeId, manifestName);
                try
                {
                    var descriptor = GetThemeDescriptor(
                        path,
                        themeId,
                        themeType,
                        manifestPath,
                        manifestIsOptional);

                    if (descriptor == null)
                        continue;
                    
                    localList.Add(descriptor);
                }
                catch (Exception)
                {
                    throw;
                }

            }

            return localList;

        }

        private IThemeDescriptor GetThemeDescriptor(
            string locationPath,
            string themeId,
            string themeType,
            string manifestPath,
            bool manifestIsOptional)
        {
            var manifestText = _fileSystem.ReadFileAsync(manifestPath).Result;
            if (manifestText == null)
            {
                if (manifestIsOptional)
                    manifestText = $"Id: {themeId}";
                else
                    return null;
            }

            return GetThemeDescriptorFromManifest(
                locationPath,
                themeId,
                themeType,
                manifestText);

        }

        private IThemeDescriptor GetThemeDescriptorFromManifest(
            string rootPath,
            string themeId,
            string themeType,
            string manifestText)
        {

            var manifest = ParseManifest(manifestText);
         
            var themeDescriptor = new ThemeDescriptor
            {
                Id = themeId,
                Name = GetValue(manifest, NameSection) ?? themeType,
                Description = GetValue(manifest, DescriptionSection),
                Version = GetValue(manifest, VersionSection),
                PlatoVersion = GetValue(manifest, PlatoVersionSection),
                Author = GetValue(manifest, AuthorSection),
                WebSite = GetValue(manifest, WebsiteSection),
                FullPath = _fileSystem.Combine(rootPath, themeId)
            };
            
            return themeDescriptor;

        }

        private static string GetValue(IDictionary<string, string> fields, string key)
        {
            return fields.TryGetValue(key, out var value) ? value : null;
        }

        private static Dictionary<string, string> ParseManifest(string manifestText)
        {
            var manifest = new Dictionary<string, string>();
            using (var reader = new StringReader(manifestText))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var field = line.Split(new[] { ":" }, 2, StringSplitOptions.None);
                    var fieldLength = field.Length;
                    if (fieldLength != 2)
                        continue;
                    for (var i = 0; i < fieldLength; i++)
                    {
                        field[i] = field[i].Trim();
                    }
                    switch (field[0].ToLowerInvariant())
                    {
                        case NameSection:
                            manifest.Add(NameSection, field[1]);
                            break;
                        case PathSection:
                            manifest.Add(PathSection, field[1]);
                            break;
                        case DescriptionSection:
                            manifest.Add(DescriptionSection, field[1]);
                            break;
                        case VersionSection:
                            manifest.Add(VersionSection, field[1]);
                            break;
                        case PlatoVersionSection:
                            manifest.Add(PlatoVersionSection, field[1]);
                            break;
                        case AuthorSection:
                            manifest.Add(AuthorSection, field[1]);
                            break;
                        case WebsiteSection:
                            manifest.Add(WebsiteSection, field[1]);
                            break;
                        case TagsSection:
                            manifest.Add(TagsSection, field[1]);
                            break;
                    }
                }
            }

            return manifest;
        }
        
        #endregion

    }

}
