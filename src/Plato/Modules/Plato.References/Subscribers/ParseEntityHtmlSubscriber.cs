using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.References.Services;

namespace Plato.References.Subscribers
{

    public class ParseEntityHtmlSubscriber : IBrokerSubscriber
    {
        
        private readonly IReferencesParser _referencesParser;
        private readonly IBroker _broker;

        public ParseEntityHtmlSubscriber(
            IReferencesParser referencesParser,
            IBroker broker)
        {
            _referencesParser = referencesParser;
            _broker = broker;
        }

        public void Subscribe()
        {
            // Add a subscription to convert @mentions to hyperlinks to user profiles
            _broker.Sub<string>(new MessageOptions()
            {
                Key = "ParseEntityHtml",
                Order = short.MaxValue // @mentions should be parsed towards the very end
            }, async message => await ParseEntityHtmlAsync(message.What));
            
        }

        public void Unsubscribe()
        {
            _broker.Unsub<string>(new MessageOptions()
            {
                Key = "ParseEntityHtml"
            }, async message => await ParseEntityHtmlAsync(message.What));

        }

        private async Task<string> ParseEntityHtmlAsync(string input)
        {
            return await _referencesParser.ParseAsync(input);
        }

    }

}
