using System.Text.Encodings.Web;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Text
{

    public class DefaultHtmlEncoder : IDefaultHtmlEncoder
    {

        // IDefaultHtmlEncoder ensures user supplied content is correctly HTML encoded
        // before being stored to the database for later display, For performance reasons
        // we saves the HTML to display within the database as opposed to parsing the
        // message everytime it's displayed. For this reason we must ensure the saved HTML
        // is correctly HTML encoded as this may be dislpayed via the Html.Raw HtmlHelper
        
        // This class offers a default implementation fo the HtmlEncoder which can be
        // override or replaced by other modules as needed

        public string Encode(string html)
        {
            // Encode the supplied html but ensure
            // we convert encoded new lines back into \n
            return HtmlEncoder.Default
                .Encode(html.NormalizeNewLines())
                .Replace("&#xA;", "\n");
        }
    }
}
