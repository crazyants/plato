using System;
using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Markdown.Services;

namespace Plato.Markdown.Subscribers
{
 
    public class ParseSignatureHtmlSubscriber : IBrokerSubscriber
    {

        private readonly IBroker _broker;
        private readonly IMarkdownParserFactory _markdownParserFactory;

        public ParseSignatureHtmlSubscriber(
            IBroker broker,
            IMarkdownParserFactory markdownParserFactory)
        {
            _broker = broker;
            _markdownParserFactory = markdownParserFactory;
        }

        public void Subscribe()
        {
            // Add a subscription to convert markdown to html
            _broker.Sub<string>(new MessageOptions()
            {
                Key = "ParseSignatureHtml",
            }, async message => await ParseSignatureHtmlAsync(message.What));

        }

        public void Unsubscribe()
        {
            _broker.Unsub<string>(new MessageOptions()
            {
                Key = "ParseSignatureHtml"
            }, async message => await ParseSignatureHtmlAsync(message.What));

        }

        private async Task<string> ParseSignatureHtmlAsync(string markdown)
        {
            var parser = _markdownParserFactory.GetParser();
            return await parser.ParseAsync(markdown);
        }
        
    }

}
