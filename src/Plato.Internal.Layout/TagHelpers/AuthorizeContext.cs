using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.TagHelpers
{
    
    public class AuthorizeContext
    {
        public AuthorizeSection Success { get; set; }

        public AuthorizeSection Fail { get; set; }
        
    }

    public class AuthorizeSection
    {
        public string CssClass { get; set; }

        public IHtmlContent Content { get; set; }

    }

}
