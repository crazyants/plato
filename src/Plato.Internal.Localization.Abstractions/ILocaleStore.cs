using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Localization.Abstractions.Models;
using Microsoft.Extensions.Localization;

namespace Plato.Internal.Localization.Abstractions
{

    public interface ILocaleStore
    {
  
        Task<IEnumerable<ComposedLocaleResource>> GetResourcesAsync(string cultureCode);

        Task<IEnumerable<LocaleValues<TModel>>> GetResourcesAsync<TModel>(string cultureCode) where TModel : class, ILocaleValue;

        Task<LocaleValues<TModel>> GetByKeyAsync<TModel>(string cultureCode, string key) where TModel : class, ILocaleValue;

        Task<IEnumerable<LocalizedString>> GetAllStringsAsync(string cultureCode);

        Task MonitorChanges();

    }

}
