using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Titles;

namespace Plato.Internal.Layout.TagHelpers
{
    [HtmlTargetElement("title", Attributes = "asp-separator")]
    public class PageTitleTagHelper : TagHelper
    {
        
        [HtmlAttributeName("asp-separator")]
        public string Separator { get; set; }
        
        private readonly IPageTitleBuilder _pageTitleBuilder;

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }


        public PageTitleTagHelper(
            IContextFacade contextFacade,
            IPageTitleBuilder pageTitleBuilder)
        {
            _pageTitleBuilder = pageTitleBuilder;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            
            var content = await output.GetChildContentAsync();
       
            output.TagName = "title";
            output.TagMode = TagMode.StartTagAndEndTag;

            var pageTitle = _pageTitleBuilder.GenerateTitle(new HtmlString(this.Separator));
            if (!Equals(pageTitle, HtmlString.Empty))
            {
                if (!content.IsEmptyOrWhiteSpace)
                {
                    output.PostContent.AppendHtml(new HtmlString(Separator));
                }
                output.PostContent.AppendHtml(pageTitle);
            }
            
            // Get updated content
            var updatedContent = await output.GetChildContentAsync();

            // Add generated title to view data for general access
            ViewContext.ViewData["PageTitle"] = updatedContent.ToHtmlString().ToString();

        }

    }

}

