using System.Threading.Tasks;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Discuss.Channels.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {
  
        public FeatureEventHandler(IBrokerSubscriber brokerSubscriber)
        {
    
        }
        
        #region "Implementation"

        public override Task InstallingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override Task InstalledAsync(IFeatureEventContext context)
        {
            // Ensure subscriptions are acttivated
       
            return Task.CompletedTask;
        }

        public override Task UninstallingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override Task UninstalledAsync(IFeatureEventContext context)
        {
            // Dispose active subscriptions
     
            return Task.CompletedTask;
        }

        #endregion
        
    }

}
