using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Discuss.Channels.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {

        private readonly IEnumerable<IBrokerSubscriber> _brokerSubscribers;

        public FeatureEventHandler(
           IEnumerable<IBrokerSubscriber> brokerSubscribers1)
        {
            _brokerSubscribers = brokerSubscribers1;
        }
        
        #region "Implementation"

        public override Task InstallingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override Task InstalledAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override Task UninstallingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override Task UninstalledAsync(IFeatureEventContext context)
        {
            // Dispose active subscriptions
            foreach (var subscriber in _brokerSubscribers)
            {
                subscriber?.Unsubscribe();
            }
            return Task.CompletedTask;
        }

        #endregion
        
    }

}
