using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.FileSystem.Abstractions;
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
        private const string OrchardVersionSection = "orchardversion";
        private const string AuthorSection = "author";
        private const string WebsiteSection = "website";
        private const string TagsSection = "tags";
        private const string AntiForgerySection = "antiforgery";
        private const string ZonesSection = "zones";
        private const string BaseThemeSection = "basetheme";
        private const string DependenciesSection = "dependencies";
        private const string CategorySection = "category";
        private const string FeatureDescriptionSection = "featuredescription";
        private const string FeatureNameSection = "featurename";
        private const string PrioritySection = "priority";
        private const string FeaturesSection = "features";
        private const string SessionStateSection = "sessionstate";

        private readonly IPlatoFileSystem _fileSystem;

        #endregion

        #region "Constructor"

        public ModuleLocator(IPlatoFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
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
                catch (Exception ex)
                {
                    throw;
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
                //Path = GetValue(manifest, PathSection),
                //Description = GetValue(manifest, DescriptionSection),
                //Version = GetValue(manifest, VersionSection),
                //OrchardVersion = GetValue(manifest, OrchardVersionSection),
                //Author = GetValue(manifest, AuthorSection),
                //WebSite = GetValue(manifest, WebsiteSection),
                //Tags = GetValue(manifest, TagsSection),
                //AntiForgery = GetValue(manifest, AntiForgerySection),
                //Zones = GetValue(manifest, ZonesSection),
                //BaseTheme = GetValue(manifest, BaseThemeSection),
                //SessionState = GetValue(manifest, SessionStateSection)
            };

            //moduleDescriptor.Features = GetFeaturesForExtension(manifest, extensionDescriptor);

            return moduleDescriptor;

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
                        case OrchardVersionSection:
                            manifest.Add(OrchardVersionSection, field[1]);
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
                            manifest.Add(DependenciesSection, field[1]);
                            break;
                        case CategorySection:
                            manifest.Add(CategorySection, field[1]);
                            break;
                        case FeatureDescriptionSection:
                            manifest.Add(FeatureDescriptionSection, field[1]);
                            break;
                        case FeatureNameSection:
                            manifest.Add(FeatureNameSection, field[1]);
                            break;
                        case PrioritySection:
                            manifest.Add(PrioritySection, field[1]);
                            break;
                        case SessionStateSection:
                            manifest.Add(SessionStateSection, field[1]);
                            break;
                        case FeaturesSection:
                            manifest.Add(FeaturesSection, reader.ReadToEnd());
                            break;
                    }
                }
            }

            return manifest;
        }

        #endregion


    }
}
