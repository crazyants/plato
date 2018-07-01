using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Markdown.Services
{

    public interface IMarkdownSubscriber
    {
        void Subscribe();
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

            //_broker.Pub<string>(this, markdownParserFactory);

            _broker.Push<string>(async message =>
            {
                var parser = _markdownParserFactory.GetParser();
                return await parser.Parse(message.What);
            });

        }

    }
}
