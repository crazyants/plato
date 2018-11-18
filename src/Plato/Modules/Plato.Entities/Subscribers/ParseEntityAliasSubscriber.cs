using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Text.Abstractions;

namespace Plato.Entities.Subscribers
{

    public class ParseEntityAliasSubscriber : IBrokerSubscriber
    {
        
        private readonly IAliasCreator _aliasCreator;
        private readonly IBroker _broker;

        public ParseEntityAliasSubscriber(
            IBroker broker,
            IAliasCreator aliasCreator)
        {
            _broker = broker;
            _aliasCreator = aliasCreator;
        }

        #region "Implementation"
        
        public void Subscribe()
        {
            _broker.Sub<string>(new MessageOptions()
            {
                Key = "ParseEntityAlias"
            }, async message => await ParseEntityAlias(message.What));
        }

        public void Unsubscribe()
        {
            _broker.Unsub<string>(new MessageOptions()
            {
                Key = "ParseEntityAlias"
            }, async message => await ParseEntityAlias(message.What));
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        #endregion

        #region "Private Methods"

        Task<string> ParseEntityAlias(string input)
        {
            return Task.FromResult(_aliasCreator.Create(input));
        }

        #endregion

    }
}
