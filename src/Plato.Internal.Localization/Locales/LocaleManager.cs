using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization.Locales
{

    public class LocaleManager : ILocaleManager
    {
        
        private readonly ILocaleProvider _localeProvider;

        public LocaleManager(ILocaleProvider localeProvider)
        {
            _localeProvider = localeProvider;
        }
    
        public async Task<IEnumerable<LocaleResourceValues<TModel>>> GetResourcesAsync<TModel>(string cultureCode) where TModel : class
        {

            var resources = new List<LocaleResourceValues<TModel>>();
            var locale = await GetResourcesAsync(cultureCode);
            if (locale != null)
            {
                foreach (var resource in locale.Resources.Where(r => r.Type == typeof(TModel)))
                {
                    resources.Add((LocaleResourceValues<TModel>) resource.Model);
                }
            }

            return resources;

        }
        
        public async Task<LocaleResources> GetResourcesAsync(string cultureCode)
        {

            var resources = new List<ComposedLocaleResource>();
            foreach (var locale in await _localeProvider.GetLocalesAsync())
            {
                if (locale.Descriptor.Name.Equals(cultureCode, StringComparison.OrdinalIgnoreCase))
                {
                    resources.AddRange(locale.Resources);
                }
            }

            if (resources.Count == 0)
            {
                return null;
            }

            return new LocaleResources()
            {
                Resources = resources
            };

        }

    }

}
