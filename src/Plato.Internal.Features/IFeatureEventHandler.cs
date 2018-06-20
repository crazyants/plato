using System.Threading.Tasks;

namespace Plato.Internal.Features
{

    public interface IFeatureEventHandler
    {
        
        Task InstallingAsync(IFeatureEventContext context);

        Task InstalledAsync(IFeatureEventContext context);

        Task UninstallingAsync(IFeatureEventContext context);

        Task UninstalledAsync(IFeatureEventContext context);

    }

}
