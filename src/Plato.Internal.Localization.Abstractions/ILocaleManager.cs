using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization.Abstractions
{

    public interface ILocaleManager
    {
  
        Task<LocaleResources> GetResourcesAsync(string cultureCode);

        Task<IEnumerable<LocaleResourceValues<TModel>>> GetResourcesAsync<TModel>(string cultureCode) where TModel : class;

    }

}
