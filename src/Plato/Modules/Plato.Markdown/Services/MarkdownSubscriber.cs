using System;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Markdown.Services
{

    public interface IMarkdownSubscriber : IDisposable
    {
        void Subscribe();

        void Unsubscribe();
    }


    public class MarkdownSubscriber : IMarkdownSubscriber
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

            // Add a subscribtion to convert markdown to html
            _broker.Sub<string>(async message =>
            {
                var parser = _markdownParserFactory.GetParser();
                return await parser.ParseAsync(message.What);
            });

        }

        public void Unsubscribe()
        {
            _broker.Unsub<string>(async message =>
            {
                var parser = _markdownParserFactory.GetParser();
                return await parser.ParseAsync(message.What);
            });
        }

        public void Dispose()
        {
            Unsubscribe();
        }
    }
}
