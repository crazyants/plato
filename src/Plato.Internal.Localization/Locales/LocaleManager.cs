using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization.Locales
{

    public class LocaleManager : ILocaleManager
    {
        
        private readonly ILocaleProvider _localeProvider;
        private readonly ICacheManager _cacheManager;

        public LocaleManager(
            ILocaleProvider localeProvider,
            ICacheManager cacheManager)
        {
            _localeProvider = localeProvider;
            _cacheManager = cacheManager;
        }

        public async Task<IEnumerable<ComposedLocaleResource>> GetResourcesAsync(string cultureCode)
        {

            var token = _cacheManager.GetOrCreateToken(this.GetType(), cultureCode);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                // Combine all composed resources from locale matching our cultureCode
                var resources = new List<ComposedLocaleResource>();
                foreach (var locale in await _localeProvider.GetLocalesAsync())
                {
                    if (locale.Descriptor.Name.Equals(cultureCode, StringComparison.OrdinalIgnoreCase))
                    {
                        resources.AddRange(locale.Resources);
                    }
                }

                return resources;

            });

        }


        public async Task<IEnumerable<LocaleValues<TModel>>> GetResourcesAsync<TModel>(string cultureCode) where TModel : class, ILocaleValue
        {

            var token = _cacheManager.GetOrCreateToken(this.GetType(), typeof(TModel), cultureCode);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                // Combine all locale values of a given type
                var localeValues = new List<LocaleValues<TModel>>();
                var locale = await GetResourcesAsync(cultureCode);
                if (locale != null)
                {
                    foreach (var resource in locale.Where(r => r.Type == typeof(TModel)))
                    {
                        localeValues.Add((LocaleValues<TModel>)resource.Model);
                    }
                }

                return localeValues;

            });

        }

        public async Task<LocaleValues<TModel>> GetByKeyAsync<TModel>(string cultureCode, string key) where TModel : class, ILocaleValue
        {

            var token = _cacheManager.GetOrCreateToken(this.GetType(), typeof(TModel), key, cultureCode);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                var localValues = await GetResourcesAsync<TModel>(cultureCode);
                var result = new LocaleValues<TModel>();
                foreach (var localValue in localValues)
                {
                    foreach (var value in localValue.Values)
                    {
                        if (value.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                        {
                            result = localValue;
                            break;
                        }
                    }
                }

                return result;

            });
        }

   
    }

}
