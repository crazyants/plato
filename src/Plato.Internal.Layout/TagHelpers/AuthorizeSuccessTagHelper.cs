using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Plato.Internal.Layout.TagHelpers
{
  
    [HtmlTargetElement("authorize-success")]
    public class AuthorizeSuccessTagHelper : TagHelper
    {

        public string Class { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = await output.GetChildContentAsync();
            var authorizeContext = (AuthorizeContext)context.Items[typeof(AuthorizeContext)];
            authorizeContext.Success = new AuthorizeSection()
            {
                CssClass = Class,
                Content = childContent
            };
            output.SuppressOutput();
        }
    }



}
