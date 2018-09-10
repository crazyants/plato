using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization.Locales
{

    public class LocaleStore : ILocaleStore
    {
        
        private readonly ILocaleProvider _localeProvider;
        private readonly ICacheManager _cacheManager;

        public LocaleStore(
            ILocaleProvider localeProvider,
            ICacheManager cacheManager)
        {
            _localeProvider = localeProvider;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Returns all locale resource types matching the supplied culture.
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ComposedLocaleResource>> GetResourcesAsync(string cultureCode)
        {

            if (String.IsNullOrEmpty(cultureCode))
            {
                throw new ArgumentNullException(nameof(cultureCode));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), cultureCode);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                
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

        /// <summary>
        /// Returns all locale values of a given type matching the supplied culture.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="cultureCode"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LocaleValues<TModel>>> GetResourcesAsync<TModel>(string cultureCode) where TModel : class, ILocaleValue
        {

            if (String.IsNullOrEmpty(cultureCode))
            {
                throw new ArgumentNullException(nameof(cultureCode));
            }
            
            var token = _cacheManager.GetOrCreateToken(typeof(TModel), cultureCode);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                
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

        /// <summary>
        /// Returns a locale value of a given type and given key matching the supplied culture.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="cultureCode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<LocaleValues<TModel>> GetByKeyAsync<TModel>(string cultureCode, string key) where TModel : class, ILocaleValue
        {

            if (String.IsNullOrEmpty(cultureCode))
            {
                throw new ArgumentNullException(nameof(cultureCode));
            }

            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var token = _cacheManager.GetOrCreateToken(typeof(TModel), key, cultureCode);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                
                var result = new LocaleValues<TModel>();
                foreach (var localeValue in await GetResourcesAsync<TModel>(cultureCode))
                {
                    foreach (var value in localeValue.Values)
                    {
                        if (value.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                        {
                            result = localeValue;
                            break;
                        }
                    }
                }

                return result;

            });

        }

        /// <summary>
        /// Returns all LocalizedStrings matching the supplied culture.
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <param name="includeParentCultures"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LocalizedString>> GetAllStringsAsync(string cultureCode, bool includeParentCultures = false)
        {

            if (String.IsNullOrEmpty(cultureCode))
            {
                throw new ArgumentNullException(nameof(cultureCode));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), cultureCode, includeParentCultures);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                var localizedStrings = new List<LocalizedString>();
                foreach (var localeValue in await GetResourcesAsync<LocaleString>(cultureCode))
                {
                    localizedStrings.AddRange(localeValue.Values.Select(item =>
                        new LocalizedString(item.Key, item.Value, true)));
                }

                return localizedStrings;

            });

        }
    }

}
