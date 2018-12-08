namespace Plato.Markdown.Services
{

    public class MarkdownParserFactory : IMarkdownParserFactory
    {

        private static IMarkdownParser _currentParser;
        
        public IMarkdownParser GetParser()
        {
            if (_currentParser != null)
                return _currentParser;
            _currentParser = new MarkdownParserMarkdig(false, null);
            return _currentParser;
        }

    }

}
