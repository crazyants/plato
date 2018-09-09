using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization.Abstractions
{

    public interface ILocaleManager
    {
  
        Task<IEnumerable<ComposedLocaleResource>> GetResourcesAsync(string cultureCode);

        Task<IEnumerable<LocaleValues<TModel>>> GetResourcesAsync<TModel>(string cultureCode) where TModel : class, ILocaleValue;

        Task<LocaleValues<TModel>> GetByKeyAsync<TModel>(string cultureCode, string key) where TModel : class, ILocaleValue;

    }

}
