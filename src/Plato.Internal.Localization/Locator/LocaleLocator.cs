using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Localization.Resources;
using Plato.Internal.Yaml.Extensions;


namespace Plato.Internal.Localization.Locator
{
    
    public class LocaleLocator : ILocaleLocator
    {
        
        private readonly IPlatoFileSystem _fileSystem;
        private readonly ILogger<LocaleLocator> _logger;
   
        public LocaleLocator(
            IPlatoFileSystem fileSystem,
            ILogger<LocaleLocator> logger, 
            ILocaleCompositionStrategy localeCompositionStrategy)
        {
            _fileSystem = fileSystem;
            _logger = logger;
        }
        
        #region "Implementation"
        
        public async Task<IEnumerable<LocaleDescriptor>> LocateLocalesAsync(IEnumerable<string> paths)
        {

            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }

            var descriptors = new List<LocaleDescriptor>();
            foreach (var path in paths)
            {
                descriptors.AddRange(await AvailableLocales(path));
            }

            return descriptors;

        }
        
        #endregion

        #region "Private Methods"

        async Task<IEnumerable<LocaleDescriptor>> AvailableLocales(string path)
        {
            var locales = await AvailableLocalesInFolder(path);
            return locales.ToReadOnlyCollection();
        }

        async Task<IList<LocaleDescriptor>> AvailableLocalesInFolder(string path)
        {
            var localList = new List<LocaleDescriptor>();

            if (string.IsNullOrEmpty(path))
            {
                return localList;
            }

            var subfolders = _fileSystem.ListDirectories(path);
            foreach (var subfolder in subfolders)
            {

                var localeId = subfolder.Name;
                var localePath = _fileSystem.Combine(path, localeId);
                try
                {
                    var descriptor = await GetLocaleDescriptorAsync(
                        localeId,
                        localePath);
                    if (descriptor == null)
                        continue;
                    descriptor.DirectoryInfo = subfolder;
                    localList.Add(descriptor);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred whilst reading a locale file for locale '{localeId}' at '{localePath}'.");
                }

            }

            return localList;

        }

        async Task<LocaleDescriptor> GetLocaleDescriptorAsync(
            string localeId,
            string localePath)
        {

            var resources = new List<LocaleResource>();
            var files = _fileSystem.ListFiles(localePath);
            foreach (var file in files)
            {

                var filePath = _fileSystem.Combine(localePath, file.Name);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.Name);

                var resource = new LocaleResource()
                {
                    Name = file.Name,
                    Path = filePath,
                    FileInfo = file,
                    Contents = await _fileSystem.ReadFileAsync(filePath)
                };
                resources.Add(resource);
            }

            return new LocaleDescriptor()
            {
                Name = localeId,
                Path = localePath,
                Resources = resources
            };

        }

        #endregion
        
    }

}
