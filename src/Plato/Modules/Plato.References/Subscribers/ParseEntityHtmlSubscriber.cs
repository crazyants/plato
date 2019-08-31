using System.Threading.Tasks;
using Plato.Internal.Abstractions.Extensions;
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
        
            _broker.Sub<string>(new MessageOptions()
            {
                Key = "ParseEntityHtml",
                Order = short.MaxValue // should be parsed towards the very end
            }, async message => await ParseEntityHtmlAsync(message.What));
            
            _broker.Sub<string>(new MessageOptions()
            {
                Key = "ParseEntityAbstract",
                Order = short.MaxValue // should be parsed towards the very end
            }, async message => await ParseEntityAbstractAsync(message.What));

        }

        public void Unsubscribe()
        {
            _broker.Unsub<string>(new MessageOptions()
            {
                Key = "ParseEntityHtml"
            }, async message => await ParseEntityHtmlAsync(message.What));

            _broker.Unsub<string>(new MessageOptions()
            {
                Key = "ParseEntityAbstract"
            }, async message => await ParseEntityAbstractAsync(message.What));

        }

        private async Task<string> ParseEntityHtmlAsync(string input)
        {
            return await _referencesParser.ParseAsync(input);
        }

        private async Task<string> ParseEntityAbstractAsync(string input)
        {
            var html = await ParseEntityHtmlAsync(input);
            return html.PlainTextulize().TrimToAround(225);
        }

    }

}
