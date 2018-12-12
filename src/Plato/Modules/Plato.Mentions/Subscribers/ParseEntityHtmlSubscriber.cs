using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Mentions.Services;

namespace Plato.Mentions.Subscribers
{

    public class ParseEntityHtmlSubscriber : IBrokerSubscriber
    {

        private readonly IBroker _broker;
        private readonly IMentionsParser _mentionParser;

        public ParseEntityHtmlSubscriber(
            IBroker broker,
            IMentionsParser mentionParser)
        {
            _broker = broker;
            _mentionParser = mentionParser;
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
            return await _mentionParser.ParseAsync(input);
        }

    }

}
