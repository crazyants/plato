using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization.Locales
{

    public class LocaleManager : ILocaleManager
    {

        private static IEnumerable<LocaleDescriptor> _localeDescriptors;
        private static IEnumerable<ComposedLocaleDescriptor> _composedLocaleDescriptors;

        private readonly ILocaleLocator _localeLocator;
        private readonly ILocaleCompositionStrategy _compositionStrategy;

        public LocaleManager(
            ILocaleLocator localeLocator,
            ILocaleCompositionStrategy compositionStrategy)
        {
            _localeLocator = localeLocator;
            _compositionStrategy = compositionStrategy;
        }
    
        public async Task<IEnumerable<ComposedLocaleDescriptor>> Locales(IEnumerable<string> paths = null)
        {

            // Ensure local descriptors are only composed once 
            if (_composedLocaleDescriptors == null)
            {

                // Compose locales
                var output = new List<ComposedLocaleDescriptor>();
                foreach (var localeDescriptor in await GetAvailableLocales(paths))
                {
                    output.Add(await _compositionStrategy.ComposeDescriptorAsync(localeDescriptor));
                }

                _composedLocaleDescriptors = output;

            }


            return _composedLocaleDescriptors;

        }
        


        async Task<IEnumerable<LocaleDescriptor>> GetAvailableLocales(IEnumerable<string> paths = null)
        {
            if (paths == null)
            {
                paths = new[] {"Locales"};
            }

            // Ensure local descriptors are only loaded once 
            return _localeDescriptors ?? (_localeDescriptors = await _localeLocator.LocateLocalesAsync(paths));
        }

    }
}
