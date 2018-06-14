using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Models.Modules;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Modules.Models;

namespace Plato.Internal.Modules.Locator
{
    public class ModuleLocator : IModuleLocator
    {

        #region "Private Memberrs"

        private const string NameSection = "name";
        private const string PathSection = "path";
        private const string DescriptionSection = "description";
        private const string VersionSection = "version";
        private const string PlatoVersionSection = "platoversion";
        private const string AuthorSection = "author";
        private const string WebsiteSection = "website";
        private const string TagsSection = "tags";
        private const string AntiForgerySection = "antiforgery";
        private const string ZonesSection = "zones";
        private const string BaseThemeSection = "basetheme";
        private const string DependenciesSection = "dependencies";
   


        private readonly IPlatoFileSystem _fileSystem;
        private readonly ILogger<ModuleLocator> _logger;

        #endregion

        #region "Constructor"

        public ModuleLocator(IPlatoFileSystem fileSystem, ILogger<ModuleLocator> logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        public async Task<IEnumerable<IModuleDescriptor>> LocateModulesAsync(
            IEnumerable<string> paths, 
            string extensionType, 
            string manifestName, 
            bool manifestIsOptional)
        {

            if (paths == null)            
                throw new ArgumentNullException(nameof(paths));
            
            var descriptors = new List<ModuleDescriptor>();
            foreach (var path in paths)
            {
                descriptors.AddRange(
                    await AvailableModules(
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

        private async Task<IEnumerable<ModuleDescriptor>> AvailableModules(string path, string extensionType, string manifestName, bool manifestIsOptional)
        {
            var modules = await AvailableModulesInFolder(
                path,
                extensionType,
                manifestName,
                manifestIsOptional);
            return modules.ToReadOnlyCollection();

        }
           
        private async Task<IList<ModuleDescriptor>> AvailableModulesInFolder(
            string path, 
            string moduleType, 
            string manifestName, 
            bool manifestIsOptional)
        {
            var localList = new List<ModuleDescriptor>();

            if (string.IsNullOrEmpty(path))
            {
                return localList;
            }
                      
            var subfolders = _fileSystem.ListDirectories(path);
            foreach (var subfolder in subfolders)
            {
           
                var moduleId = subfolder.Name;
                var manifestPath = _fileSystem.Combine(path, moduleId, manifestName);
                try
                {
                    var descriptor = await GetModuleDescriptorAsync(
                        path,
                        moduleId,
                        moduleType,
                        manifestPath,
                        manifestIsOptional);

                    if (descriptor == null)
                        continue;

                    if (descriptor.Location == null)
                    {
                        descriptor.Location = descriptor.Name.IsValidUrlSegment()
                                              ? descriptor.Name
                                              : descriptor.Id;
                    }

                    localList.Add(descriptor);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred whilst parsing the manifest file at {path}");
                }

            }
        
            return localList;

        }

        private async Task<ModuleDescriptor> GetModuleDescriptorAsync(
            string locationPath, 
            string moduleId, 
            string moduleType, 
            string manifestPath, 
            bool manifestIsOptional)
        {
            var manifestText = await _fileSystem.ReadFileAsync(manifestPath);
            if (manifestText == null)
            {
                if (manifestIsOptional)                
                    manifestText = $"Id: {moduleId}";                
                else                
                    return null;                
            }

            return GetModuleDescriptorFromManifest(
                locationPath, 
                moduleId, 
                moduleType, 
                manifestText);

        }

        private ModuleDescriptor GetModuleDescriptorFromManifest(
            string rootPath, 
            string moduleId, 
            string moduleType, 
            string manifestText)
        {

            var manifest = ParseManifest(manifestText);            
            var virtualPathToBin = _fileSystem.Combine(rootPath, moduleId, "Bin");
      
            var moduleDescriptor = new ModuleDescriptor
            {
                Location = rootPath,
                VirtualPathToBin = virtualPathToBin.Replace("/", "\\"),
                Id = moduleId,
                ModuleType = moduleType,
                Name = GetValue(manifest, NameSection) ?? moduleId,
                Path = GetValue(manifest, PathSection),
                Description = GetValue(manifest, DescriptionSection),
                Version = GetValue(manifest, VersionSection),
                PlatoVersion = GetValue(manifest, PlatoVersionSection),
                Author = GetValue(manifest, AuthorSection),
                WebSite = GetValue(manifest, WebsiteSection),
                Tags = GetValue(manifest, TagsSection),
            };

            moduleDescriptor.Dependencies = GetModuleDependencies(manifest, DependenciesSection);

            return moduleDescriptor;

        }

        public IEnumerable<ModuleDependency> GetModuleDependencies(IDictionary<string, string> fields, string key)
        {

            List<ModuleDependency> output = null;
            var dependencies =  fields.TryGetValue(key, out var value) ? value : null;
            if (value != null)
            {
                try
                {
                    output = value.Deserialize<List<ModuleDependency>>();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred parsing the dependency JSON string within your manifest.text file at { GetValue(fields, PathSection)}");
                }
            }

            return output ?? new List<ModuleDependency>();

        }

        static string GetValue(IDictionary<string, string> fields, string key)
        {
            return fields.TryGetValue(key, out var value) ? value : null;
        }

        static Dictionary<string, string> ParseManifest(string manifestText)
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
                        case AntiForgerySection:
                            manifest.Add(AntiForgerySection, field[1]);
                            break;
                        case ZonesSection:
                            manifest.Add(ZonesSection, field[1]);
                            break;
                        case BaseThemeSection:
                            manifest.Add(BaseThemeSection, field[1]);
                            break;
                        case DependenciesSection:
                            manifest.Add(DependenciesSection, reader.ReadToEnd());
                            break;
                    }
                }
            }

            return manifest;
        }

        #endregion


    }
}
