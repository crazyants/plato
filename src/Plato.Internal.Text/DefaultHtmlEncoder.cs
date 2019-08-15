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
        // message every time it's displayed. For this reason we must ensure the saved HTML
        // is correctly HTML encoded as this may be displayed via the Html.Raw HtmlHelper
        
        // This class offers a default implementation fo the HtmlEncoder which can be
        // overriden or replaced by modules as needed

        public string Encode(string html)
        {

            // We always need a string to encode
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }

            // Encode the supplied html but ensure
            // we convert encoded new lines back into \n
            // then convert all \n to <br/>
            return HtmlEncoder.Default
                .Encode(html.NormalizeNewLines())
                .Replace("&#xA;", "\n")
                .HtmlTextulize();
        }
    }
}
