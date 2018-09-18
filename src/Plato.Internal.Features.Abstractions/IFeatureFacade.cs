using System.Threading.Tasks;
using Plato.Internal.Models.Features;
using Plato.Internal.Models.Shell;

namespace Plato.Internal.Features.Abstractions
{
    public interface IFeatureFacade
    {
        
        Task<IShellFeature> GetFeatureByIdAsync(string moduleId);
    }

}
