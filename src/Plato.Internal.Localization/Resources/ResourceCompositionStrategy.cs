using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Localization.Serializers;
using Plato.Internal.Yaml.Extensions;

namespace Plato.Internal.Localization.Resources
{

    public class LocaleCompositionStrategy : ILocaleCompositionStrategy
    {
        private readonly IPlatoFileSystem _fileSystem;
        private readonly ILogger<LocaleCompositionStrategy> _logger;

        public LocaleCompositionStrategy(
            IPlatoFileSystem fileSystem, 
            ILogger<LocaleCompositionStrategy> logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
        }

        public async Task<ComposedLocaleDescriptor> ComposeDescriptorAsync(LocaleDescriptor descriptor)
        {

            var resources = new List<ComposedLocaleResource>();
            foreach (var resource in descriptor.Resources)
            {
                resources.Add(await ComposeResourceAsync(resource));
            }

            return new ComposedLocaleDescriptor
            {
                Descriptor = descriptor,
                Resources = resources
            };

        }
        
        Task<ComposedLocaleResource> ComposeResourceAsync(LocaleResource resource)
        {

            var filePath = _fileSystem.Combine(resource.Path, resource.Name);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(resource.Name);

            if (String.IsNullOrEmpty(fileNameWithoutExtension))
            {
                return null;
            }

            var configurationContainer =
                new ConfigurationBuilder()
                    .SetBasePath(_fileSystem.RootPath)
                    .AddJsonFile(_fileSystem.Combine(resource.Path, fileNameWithoutExtension + ".json"), true)
                    .AddXmlFile(_fileSystem.Combine(resource.Path, fileNameWithoutExtension + ".xml"), true)
                    .AddYamlFile(_fileSystem.Combine(resource.Path, fileNameWithoutExtension + ".txt"), true);

            var config = configurationContainer.Build();

            var composedLocaleResource = new ComposedLocaleResource
            {
                LocaleResource = resource
            };

            switch (fileNameWithoutExtension.ToLower())
            {
                case "resources":
                {
                    composedLocaleResource.Compose<KeyValuePair>(model => KeyValuePairSerializer.Parse(config));
                    break;
                }
                case "emails":
                {
                    composedLocaleResource.Compose<EmailTemplate>(model => EmailSerializer.Parse(config));
                    break;
                }
            }

            return Task.FromResult(composedLocaleResource);


        }

    }
}
