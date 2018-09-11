using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization.Locales
{

    public class LocaleStore : ILocaleStore
    {
   
        private readonly ILocaleProvider _localeProvider;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<LocaleStore> _logger;

        public LocaleStore(
            ILocaleProvider localeProvider,
            ICacheManager cacheManager,
            IPlatoFileSystem fileSystem,
            ILogger<LocaleStore> logger)
        {
            _localeProvider = localeProvider;
            _cacheManager = cacheManager;
            _logger = logger;
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
            
            var token = _cacheManager.GetOrCreateToken(typeof(ComposedLocaleResource), cultureCode);
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
        /// Returns all LocalizedStrings matching the supplied culture.
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LocalizedString>> GetAllStringsAsync(string cultureCode)
        {

            if (String.IsNullOrEmpty(cultureCode))
            {
                throw new ArgumentNullException(nameof(cultureCode));
            }
            
            var token = _cacheManager.GetOrCreateToken(typeof(LocalizedString), cultureCode);
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


        /// <summary>
        /// Returns all locale values of a given type matching the supplied culture.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="cultureCode"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LocaleValues<TModel>>> GetResourcesAsync<TModel>(string cultureCode)
            where TModel : class, ILocaleValue
        {

            if (String.IsNullOrEmpty(cultureCode))
            {
                throw new ArgumentNullException(nameof(cultureCode));
            }

            var localeValues = new List<LocaleValues<TModel>>();
            var locale = await GetResourcesAsync(cultureCode);
            if (locale != null)
            {
                foreach (var resource in locale.Where(r => r.Type == typeof(TModel)))
                {
                    localeValues.Add((LocaleValues<TModel>) resource.Model);
                }
            }

            return localeValues;


        }
        
        /// <summary>
        /// Returns a locale value of a given type and given key matching the supplied culture.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="cultureCode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<LocaleValues<TModel>> GetByKeyAsync<TModel>(string cultureCode, string key)
            where TModel : class, ILocaleValue
        {

            if (String.IsNullOrEmpty(cultureCode))
            {
                throw new ArgumentNullException(nameof(cultureCode));
            }

            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            foreach (var localeValue in await GetResourcesAsync<TModel>(cultureCode))
            {
                foreach (var value in localeValue.Values)
                {
                    if (value.Key == key)
                    {
                        return localeValue;
                    }
                }
            }

            return null;

        }

        public async Task<TModel> GetFirstOrDefaultByKeyAsync<TModel>(string cultureCode, string key)
            where TModel : class, ILocaleValue
        {
            var emails = await GetByKeyAsync<TModel>(cultureCode, key);
            return emails?.Values.FirstOrDefault();
        }

        /// <summary>
        /// Froms all locale resources from in-memory cache.
        /// </summary>
        /// <returns></returns>
        public async Task ClearCache()
        {

            var locales = await _localeProvider.GetLocalesAsync();
            if (locales != null)
            {
                foreach (var locale in locales)
                {
                    _cacheManager.CancelTokens(typeof(ComposedLocaleResource), locale.Descriptor.Name);
                    _cacheManager.CancelTokens(typeof(LocalizedString), locale.Descriptor.Name);

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Deleted cache for locale '{0}' located at '{1}'",
                            locale.Descriptor.Name, locale.Descriptor.DirectoryInfo.FullName);
                    }

                }
            }
            
        }
        

    }

}
