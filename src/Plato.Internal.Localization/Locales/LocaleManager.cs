using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization.Locales
{

    public class LocaleManager : ILocaleManager
    {

        private static IEnumerable<LocaleDescriptor> _localeDescriptors;

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

            var output = new List<ComposedLocaleDescriptor>();
            foreach (var localeDescriptor in await AvailableLocales(paths))
            {
                output.Add(await _compositionStrategy.ComposeDescriptorAsync(localeDescriptor));
            }
            
            return output;

        }
        
        async Task<IEnumerable<LocaleDescriptor>> AvailableLocales(IEnumerable<string> paths = null)
        {
            if (paths == null)
            {
                paths = new[] {"Locales"};
            }

            return _localeDescriptors ?? (_localeDescriptors = await _localeLocator.LocateLocalesAsync(paths));
        }

    }
}
