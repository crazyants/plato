using System;
using Plato.Internal.Abstractions;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Theming.Abstractions;
using Plato.Internal.Theming.Abstractions.Models;
using Plato.Internal.Yaml;

namespace Plato.Internal.Theming
{

    public class ThemeDescriptorUpdater : IThemeDescriptorUpdater
    {
        private const string ByThemeFileNameFormat = "Theme.{0}";

        private readonly IPlatoFileSystem _platoFileSystem;

        public ThemeDescriptorUpdater(IPlatoFileSystem platoFileSystem)
        {
            _platoFileSystem = platoFileSystem;
        }

        public ICommandResult<IThemeDescriptor> UpdateThemeDescriptor(
            string pathToThemeFolder,
            IThemeDescriptor descriptor)
        {

            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (string.IsNullOrEmpty(descriptor.Name))
            {
                throw new ArgumentNullException(nameof(descriptor.Name));
            }

            // Path to theme manifest file
            var fileName = string.Format(ByThemeFileNameFormat, "txt");
            var manifestPath = _platoFileSystem.MapPath(
                _platoFileSystem.Combine(pathToThemeFolder, fileName));

            // Configure YAML configuration
            var configurationProvider = new YamlConfigurationProvider(new YamlConfigurationSource
            {
                Path = manifestPath,
                Optional = false
            });

            // Build configuration
            foreach (var key in descriptor.Keys)
            {
                if (!string.IsNullOrEmpty(descriptor[key]))
                {
                    configurationProvider.Set(key, descriptor[key]);
                }
            }

            var result = new CommandResult<ThemeDescriptor>();

            try
            {
                configurationProvider.Commit();
            }
            catch (Exception e)
            {
                return result.Failed(e.Message);
            }

            return result.Success(descriptor);

        }
        
    }

}
