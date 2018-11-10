using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Mentions.Services;

namespace Plato.Mentions.Subscribers
{

    public class MentionsSubscriber : IBrokerSubscriber
    {

        private readonly IBroker _broker;
        private readonly IMentionsParser _mentionParser;

        public MentionsSubscriber(
            IBroker broker,
            IMentionsParser mentionParser)
        {
            _broker = broker;
            _mentionParser = mentionParser;
        }

        public void Subscribe()
        {
            // Add a subscription to convert markdown to html
            _broker.Sub<string>(new MessageOptions()
            {
                Key = "ParseMarkdown"
            }, async message => await ParseMarkdownAsync(message.What));
            
        }

        public void Unsubscribe()
        {
            _broker.Unsub<string>(new MessageOptions()
            {
                Key = "ParseMarkdown"
            }, async message => await ParseMarkdownAsync(message.What));

        }

        private async Task<string> ParseMarkdownAsync(string markdown)
        {
            return await _mentionParser.ParseAsync(markdown);
        }

    }

}
