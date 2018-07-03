using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Plato.Internal.Layout.TagHelpers
{
    [HtmlTargetElement("card-footer", ParentTag = "card")]
    public class CardFooterTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = await output.GetChildContentAsync();
            var modalContext = (CardContext)context.Items[typeof(CardContext)];
            modalContext.Footer = childContent;
            output.SuppressOutput();
        }
    }
}
