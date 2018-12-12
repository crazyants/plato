using Plato.Internal.Text.Abstractions;

namespace Plato.Markdown.Services
{
    public class MarkdownHtmlEncoder : IDefaultHtmlEncoder
    {
        public string Encode(string html)
        {
            // No need to HtmlEncode our HTML if the markdown feature is enabled
            // It's the markdown parsers responsibility to safely parse and encode the HTML
            return html;
        }
    }
}
