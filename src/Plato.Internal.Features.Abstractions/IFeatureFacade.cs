using System.Threading.Tasks;
using Plato.Internal.Models.Features;
using Plato.Internal.Models.Shell;

namespace Plato.Internal.Features.Abstractions
{
    public interface IFeatureFacade
    {

        Task<IShellModule> GetModuleByIdAsync(string moduleId);

        Task<IShellFeature> GetFeatureByIdAsync(string moduleId);
    }

}
