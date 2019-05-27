using System.Threading.Tasks;
using Plato.Internal.Models.Features;

namespace Plato.Internal.Features.Abstractions
{
    public interface IFeatureEventManager
    {

        Task<IFeatureEventContext> InstallingAsync(IFeatureEventContext context);

        Task<IFeatureEventContext> InstalledAsync(IFeatureEventContext context);

        Task<IFeatureEventContext> UninstallingAsync(IFeatureEventContext context);

        Task<IFeatureEventContext> UninstalledAsync(IFeatureEventContext context);

        Task<IFeatureEventContext> UpdatingAsync(IFeatureEventContext context);

        Task<IFeatureEventContext> UpdatedAsync(IFeatureEventContext context);

    }

}
