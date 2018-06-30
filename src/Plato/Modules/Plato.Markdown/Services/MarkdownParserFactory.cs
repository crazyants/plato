using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plato.Markdown.Services
{

    public interface IMarkdownParser
    {
        Task<string> Parse(string markdown);

    }

    public interface IMarkdownParserFactory
    {
        IMarkdownParser GetParser();
    }

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
