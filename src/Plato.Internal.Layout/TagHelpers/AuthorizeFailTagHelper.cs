using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Plato.Internal.Layout.TagHelpers
{

    [HtmlTargetElement("authorize-fail")]
    public class AuthorizeFailTagHelper : TagHelper
    {

        public string Class { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            // Suppress tag
            output.TagName = "";
            output.TagMode = TagMode.StartTagAndEndTag;

            var childContent = await output.GetChildContentAsync();
            var authorizeContext = (AuthorizeContext)context.Items[typeof(AuthorizeContext)];
            authorizeContext.Fail = new AuthorizeSection()
            {
                CssClass = Class,
                Content = childContent
            };
            output.SuppressOutput();
        }

    }
    
}
