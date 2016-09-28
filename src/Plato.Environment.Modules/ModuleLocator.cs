using System;
using System.Collections.Generic;
using Plato.FileSystem;
using System.IO;
using Plato.Utility;
using Plato.Abstractions.Utility.Extensions;

namespace Plato.Environment.Modules
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

        public IEnumerable<ModuleDescriptor> LocateModuless(
            IEnumerable<string> paths, 
            string extensionType, 
            string manifestName, 
            bool manifestIsOptional)
        {

            if (paths == null)            
                throw new ArgumentNullException(nameof(paths));
            
            var descriptors = new List<ModuleDescriptor>();
            foreach (string path in paths)
            {
                descriptors.AddRange(
                    AvailableModules(
                        path, extensionType,
                        manifestName, 
                        manifestIsOptional)
                    );
            }

            return descriptors;

        }

        #endregion

        #region "Private Methods"

        private IEnumerable<ModuleDescriptor> AvailableModules(string path, string extensionType, string manifestName, bool manifestIsOptional)
        {
            return AvailableModulesInFolder(
                path, 
                extensionType, 
                manifestName, 
                manifestIsOptional).ToReadOnlyCollection();

        }
           
        private List<ModuleDescriptor> AvailableModulesInFolder(
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
                    var descriptor = GetModuleDescriptor(
                        path, 
                        moduleId, 
                        moduleType, 
                        manifestPath, 
                        manifestIsOptional);

                    if (descriptor == null)
                        continue;

                    if (descriptor.Path != null && !descriptor.Path.IsValidUrlSegment())
                    {
                        //_logger.LogError("The module '{0}' could not be loaded because it has an invalid Path ({1}). It was ignored. The Path if specified must be a valid URL segment. The best bet is to stick with letters and numbers with no spaces.",
                        //             extensionId,
                        //             descriptor.Path);
                        continue;
                    }

                    if (descriptor.Path == null)
                    {
                        descriptor.Path = descriptor.Name.IsValidUrlSegment()
                                              ? descriptor.Name
                                              : descriptor.ID;
                    }

                    localList.Add(descriptor);
                }
                catch (Exception ex)
                {
                    throw ex;
                    // TODO: implement logging
                }
            }
        
            return localList;
        }

        private ModuleDescriptor GetModuleDescriptor(
            string locationPath, 
            string extensionId, 
            string extensionType, 
            string manifestPath, 
            bool manifestIsOptional)
        {
            var manifestText = _fileSystem.ReadFileAsync(manifestPath).Result;
            if (manifestText == null)
            {

                if (manifestIsOptional)                
                    manifestText = string.Format("Id: {0}", extensionId);                
                else                
                    return null;                
            }

            return GetDescriptorForModule(
                locationPath, 
                extensionId, 
                extensionType, 
                manifestText);

        }

        private static ModuleDescriptor GetDescriptorForModule(
            string locationPath, 
            string moduleId, 
            string moduleType, 
            string manifestText)
        {
            Dictionary<string, string> manifest = ParseManifest(manifestText);
            var moduleDescriptor = new ModuleDescriptor
            {
                //Location = locationPath,
                ID = moduleId,
                ModuleType = moduleType,
                Name = GetValue(manifest, NameSection) ?? moduleId,
                Path = GetValue(manifest, PathSection),
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
            string value;
            return fields.TryGetValue(key, out value) ? value : null;
        }

        private static Dictionary<string, string> ParseManifest(string manifestText)
        {
            var manifest = new Dictionary<string, string>();

            using (StringReader reader = new StringReader(manifestText))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] field = line.Split(new[] { ":" }, 2, StringSplitOptions.None);
                    int fieldLength = field.Length;
                    if (fieldLength != 2)
                        continue;
                    for (int i = 0; i < fieldLength; i++)
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
