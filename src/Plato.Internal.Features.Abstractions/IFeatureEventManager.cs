using System.Threading.Tasks;
using Plato.Internal.Models.Features;

namespace Plato.Internal.Features.Abstractions
{
    public interface IFeatureEventManager
    {

        Task<IFeatureEventContext> InstallingAsync(IFeatureEventContext feature);

        Task<IFeatureEventContext> InstalledAsync(IFeatureEventContext feature);

        Task<IFeatureEventContext> UninstallingAsync(IFeatureEventContext feature);

        Task<IFeatureEventContext> UninstalledAsync(IFeatureEventContext feature);
    }


}
