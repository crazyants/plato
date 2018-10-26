using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.TagHelpers
{
    public class CardContext
    {
        public CardSection Title { get; set; }

        public CardSection Body { get; set; }

        public CardSection Footer { get; set; }

    }

    public class CardSection
    {
        public string CssClass { get; set; }

        public IHtmlContent Content { get; set; }
    }

}
