using System.Threading.Tasks;
using Plato.Internal.Models.Features;

namespace Plato.Internal.Features
{
    public interface IFeatureEventManager
    {

        Task InstallingAsync(IShellFeature feature);

        Task InstalledAsync(IShellFeature feature);

        Task UninstallingAsync(IShellFeature feature);

        Task UninstalledAsync(IShellFeature feature);
    }


}
