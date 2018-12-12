using System.Text.Encodings.Web;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Text
{
    
    public class DefaultHtmlEncoder : IDefaultHtmlEncoder
    {
        public string Encode(string html)
        {
            return HtmlEncoder.Default.Encode(html);
        }
    }
}
