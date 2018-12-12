using Plato.Internal.Text.Abstractions;

namespace Plato.Markdown.Services
{
    public class MarkdownHtmlEncoder : IDefaultHtmlEncoder
    {
        public string Encode(string html)
        {
            // Defer the responsibility to the markdown parser
            // to safely encode the supplied HTML
            return html;
        }
    }
}
