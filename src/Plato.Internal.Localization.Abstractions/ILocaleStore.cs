using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Localization.Abstractions.Models;
using LocalizedString = Microsoft.Extensions.Localization.LocalizedString;

namespace Plato.Internal.Localization.Abstractions
{

    public interface ILocaleStore
    {
  
        Task<IEnumerable<ComposedLocaleResource>> GetResourcesAsync(string cultureCode);

        Task<IEnumerable<LocalizedValues<TModel>>> GetResourcesAsync<TModel>(string cultureCode) where TModel : class, ILocalizedValue;

        Task<IEnumerable<LocalizedString>> GetAllStringsAsync(string cultureCode);

        void Dispose();
        
    }

}
