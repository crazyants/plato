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

            var title = _pageTitleBuilder.GenerateTitle(new HtmlString(this.Separator));
            if (!Equals(title, HtmlString.Empty))
            {
                if (!content.IsEmptyOrWhiteSpace)
                {
                    output.PostContent.AppendHtml(new HtmlString(Separator));
                }
                output.PostContent.AppendHtml(title);

                // Add generated page title to context for general access
                ViewContext.HttpContext.Items[typeof(PageTitle)] = new PageTitle()
                {
                    Title = title.ToHtmlString().ToString()
                };

            }
          
        }

    }

}

