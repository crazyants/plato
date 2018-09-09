using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Localization.LocaleSerializers;
using Plato.Internal.Yaml.Extensions;

namespace Plato.Internal.Localization.Locales
{

    public class LocaleCompositionStrategy : ILocaleCompositionStrategy
    {

        internal const string EmailsFileName = "emails";

        private readonly IPlatoFileSystem _fileSystem;
        private readonly ILogger<LocaleCompositionStrategy> _logger;
        
        public LocaleCompositionStrategy(
            IPlatoFileSystem fileSystem, 
            ILogger<LocaleCompositionStrategy> logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
        }

        public virtual async Task<ComposedLocaleDescriptor> ComposeLocaleDescriptorAsync(LocaleDescriptor descriptor)
        {

            var resources = new List<ComposedLocaleResource>();
            foreach (var resource in descriptor.Resources)
            {
                resources.Add(await ComposeLocaleResourceAsync(resource));
            }

            return new ComposedLocaleDescriptor
            {
                Descriptor = descriptor,
                Resources = resources
            };

        }
        
        public virtual Task<ComposedLocaleResource> ComposeLocaleResourceAsync(LocaleResource resource)
        {

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(resource.Name);
            if (String.IsNullOrEmpty(fileNameWithoutExtension))
            {
                return null;
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Compsing locale file found at '{0}', attempting to load.", resource.Path);
            }

            var configurationContainer =
                new ConfigurationBuilder()
                    .SetBasePath(_fileSystem.RootPath)
                    .AddJsonFile(_fileSystem.Combine(resource.Location, fileNameWithoutExtension + ".json"), false)
                    .AddXmlFile(_fileSystem.Combine(resource.Location, fileNameWithoutExtension + ".xml"), true)
                    .AddYamlFile(_fileSystem.Combine(resource.Location, fileNameWithoutExtension + ".txt"), true);
            var config = configurationContainer.Build();

            var composedLocaleResource = new ComposedLocaleResource
            {
                LocaleResource = resource
            };

            switch (fileNameWithoutExtension.ToLower())
            {
                case EmailsFileName:
                {
                    composedLocaleResource.Configure<LocaleEmail>(model => new LocaleValues<LocaleEmail>
                    {
                        Resource = resource,
                        Values = LocaleEmailsSerializer.Parse(config)
                    });
                    break;
                }
                default:
                {
                    composedLocaleResource.Configure<LocaleString>(model => new LocaleValues<LocaleString>
                    {
                        Resource = resource,
                        Values = LocaleStringSerializer.Parse(config)
                    });
                    break;
                }
            }
            
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Completed compsing locale files found in '{0}'.", resource.Path);
            }
            
            return Task.FromResult(composedLocaleResource);

        }

    }
}
