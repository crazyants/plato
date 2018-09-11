using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Internal.Localization.Locales
{

    public class LocaleProvider : ILocaleProvider
    {

        // Default folder containing localizations
        private const string LocaleFolderName = "Locales";

        // Local storage
        private static IEnumerable<ComposedLocaleDescriptor> _composedLocaleDescriptors;
        private static IEnumerable<LocaleDescriptor> _localeDescriptors;
        
        // Dependencies
        private readonly ILocaleCompositionStrategy _compositionStrategy;
        private readonly IModuleManager _moduleManager;
        private readonly ILocaleLocator _localeLocator;

        public LocaleProvider(
            ILocaleCompositionStrategy compositionStrategy,
            IModuleManager moduleManager,
            ILocaleLocator localeLocator)
        {
            _compositionStrategy = compositionStrategy;
            _moduleManager = moduleManager;
            _localeLocator = localeLocator;
        }

        public async Task<IEnumerable<ComposedLocaleDescriptor>> GetLocalesAsync()
        {

            // Ensure local descriptors are only composed once 
            if (_composedLocaleDescriptors == null)
            {

                // Compose locales
                var output = new List<ComposedLocaleDescriptor>();
                foreach (var localeDescriptor in await GetAvailableLocaleDescriptors())
                {
                    output.Add(await _compositionStrategy.ComposeLocaleDescriptorAsync(localeDescriptor));
                }

                _composedLocaleDescriptors = output;

            }

            return _composedLocaleDescriptors;

        }

        async Task<IEnumerable<LocaleDescriptor>> GetAvailableLocaleDescriptors()
        {

            // Build paths to locale descriptors
            var paths = await GetLocaleDescriptorPathsToSearch();

            // Load descriptors or reuse if already loaded
            return _localeDescriptors ?? (_localeDescriptors = await _localeLocator.LocateLocalesAsync(paths));

        }

        public void Dispose()
        {
            _composedLocaleDescriptors = null;
            _localeDescriptors = null;
        }
        
        async Task<string[]> GetLocaleDescriptorPathsToSearch()
        {

            // Initialize with root "Locales" folder
            var pathsToSearch = new List<string>()
            {
                LocaleFolderName
            };

            // Append additional module locations to paths to search
            var moduleDescriptors = await _moduleManager.LoadModulesAsync();
            foreach (var module in moduleDescriptors)
            {
                var modulePath = module.Descriptor.VirtualPathToModule;
                if (!modulePath.EndsWith("\\"))
                {
                    modulePath += "\\";
                }
                pathsToSearch.Add(modulePath + LocaleFolderName);
            }

            return pathsToSearch.ToArray();

        }

    }

}
