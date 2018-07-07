using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Markdown.Services
{
    
    public class MarkdownSubscriber : IBrokerSubscriber
    {

        private readonly IBroker _broker;
        private readonly IMarkdownParserFactory _markdownParserFactory;

        public MarkdownSubscriber(
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
                Key = "ParseMarkdown"
            }, async message => await ParseMarkdownAsync(message.What));
            
            // Add a subscription to produce a plaintext abstract from our markdown
            _broker.Sub<string>(new MessageOptions()
            {
                Key = "ParseAbstract"
            }, async message => await ParseAbstractAsync(message.What));
            
        }

        public void Unsubscribe()
        {

            _broker.Unsub<string>(new MessageOptions()
            {
                Key = "ParseMarkdown"
            }, async message => await ParseMarkdownAsync(message.What));

            _broker.Unsub<string>(new MessageOptions()
            {
                Key = "ParseAbstract"
            }, async message => await ParseAbstractAsync(message.What));

        }
        
        private async Task<string> ParseMarkdownAsync(string markdown)
        {
            var parser = _markdownParserFactory.GetParser();
            return await parser.ParseAsync(markdown);
        }

        private async Task<string> ParseAbstractAsync(string markdown)
        {
            var html = await ParseMarkdownAsync(markdown);
            return html.StripHtml().TrimToAround(500);
        }
        
        public void Dispose()
        {
            Unsubscribe();
        }
    }
}
