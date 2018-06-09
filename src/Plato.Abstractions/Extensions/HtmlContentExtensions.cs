using Microsoft.AspNetCore.Html;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;

namespace Plato.Abstractions.Extensions
{

    public static class HtmlContentExtensions
    {

        public static HtmlString ToHtmlString(this IHtmlContent output)
        {
            using (var writer = new StringWriter())
            {
                output.WriteTo(writer, HtmlEncoder.Default);
                return new HtmlString(writer.ToString());
            }
        }
        
        public static string ToString(this IHtmlContent content)
        {
            using (var writer = new StringWriter())
            {
                content.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();
            }
        }

    }
}
