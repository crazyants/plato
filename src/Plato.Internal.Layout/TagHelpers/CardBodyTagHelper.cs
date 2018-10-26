using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Plato.Internal.Layout.TagHelpers
{
    [HtmlTargetElement("card-body", ParentTag = "card")]
    public class CardBodyTagHelper : TagHelper
    {

        public string Class { get; set; } = "card-body";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = await output.GetChildContentAsync();
            var modalContext = (CardContext)context.Items[typeof(CardContext)];
            modalContext.Body = new CardSection()
            {
                CssClass = Class,
                Content = childContent
            };
            output.SuppressOutput();
        }
    }

}
