using System.Threading.Tasks;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Discuss.Channels.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {
    
        private readonly IBrokerSubscriber _brokerSubscriber;

        public FeatureEventHandler(IBrokerSubscriber brokerSubscriber)
        {
            _brokerSubscriber = brokerSubscriber;
        }
        
        #region "Implementation"

        public override Task InstallingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override Task InstalledAsync(IFeatureEventContext context)
        {
            // Ensure subscriptions are acttivated
            _brokerSubscriber.Subscribe();
            return Task.CompletedTask;
        }

        public override Task UninstallingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override Task UninstalledAsync(IFeatureEventContext context)
        {
            // Dispose active subscriptions
            _brokerSubscriber.Dispose();
            return Task.CompletedTask;
        }

        #endregion
        
    }

}
