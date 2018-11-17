using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization.Extensions
{

    public static class LocaleStoreExtensions
    {

        public static async Task<IEnumerable<LocalizedValues<TModel>>> GetResourcesAsync<TModel>(
            this ILocaleStore localeStore, string cultureCode)
            where TModel : class, ILocalizedValue
        {

            if (String.IsNullOrEmpty(cultureCode))
            {
                throw new ArgumentNullException(nameof(cultureCode));
            }

            var localeValues = new List<LocalizedValues<TModel>>();
            var locale = await localeStore.GetResourcesAsync(cultureCode);
            if (locale != null)
            {
                foreach (var resource in locale.Where(r => r.Type == typeof(TModel)))
                {
                    localeValues.Add((LocalizedValues<TModel>)resource.Model);
                }
            }

            return localeValues;


        }
        public static async Task<LocalizedValues<TModel>> GetByKeyAsync<TModel>(
            this ILocaleStore localeStore,  string cultureCode, string key)
            where TModel : class, ILocalizedValue
        {

            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            foreach (var localeValue in await localeStore.GetResourcesAsync<TModel>(cultureCode))
            {
                foreach (var value in localeValue.Values)
                {
                    if (value.Name == key)
                    {
                        return new LocalizedValues<TModel>(localeValue.Resource, new List<TModel>()
                        {
                            value
                        });
                    }
                }
            }

            return null;

        }

        public static async Task<TModel> GetFirstOrDefaultByKeyAsync<TModel>(
            this ILocaleStore localeStore,  string cultureCode, string key)
            where TModel : class, ILocalizedValue
        {
            var localeValues = await localeStore.GetByKeyAsync<TModel>(cultureCode, key);
            return localeValues?.Values.FirstOrDefault();
        }
        
    }

}
