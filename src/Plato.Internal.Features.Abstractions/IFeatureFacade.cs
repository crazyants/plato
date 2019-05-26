using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Features;

namespace Plato.Internal.Features.Abstractions
{
    public interface IFeatureFacade
    {
        
        Task<IShellFeature> GetFeatureByIdAsync(string moduleId);

        Task<IEnumerable<IShellFeature>> GetFeatureUpdatesAsync();
        
    }

}
