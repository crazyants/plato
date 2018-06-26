using System.Threading.Tasks;

namespace Plato.Internal.Features.Abstractions
{

    public interface IFeatureEventHandler
    {

        string Id { get; }
        
        Task InstallingAsync(IFeatureEventContext context);

        Task InstalledAsync(IFeatureEventContext context);

        Task UninstallingAsync(IFeatureEventContext context);

        Task UninstalledAsync(IFeatureEventContext context);

    }

}
