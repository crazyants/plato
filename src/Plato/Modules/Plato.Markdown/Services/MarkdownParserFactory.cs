namespace Plato.Markdown.Services
{

    public class MarkdownParserFactory : IMarkdownParserFactory
    {

        private static IMarkdownParser _currentParser;
        
        public IMarkdownParser GetParser()
        {

            var usePragmaLines = false;
         
            if (_currentParser != null)
                return _currentParser;

            _currentParser = new MarkdownParserMarkdig(usePragmaLines, null);
            return _currentParser;
        }

    }
}
