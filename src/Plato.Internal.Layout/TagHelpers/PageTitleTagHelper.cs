using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Titles;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{
    [HtmlTargetElement("title", Attributes = "asp-separator")]
    public class PageTitleTagHelper : TagHelper
    {
        
        [HtmlAttributeName("asp-separator")]
        public string Separator { get; set; }
        
        private readonly IPageTitleBuilder _pageTitleBuilder;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IActionContextAccessor _actionContextAccessor;

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }
        
        public PageTitleTagHelper(
            IContextFacade contextFacade,
            IPageTitleBuilder pageTitleBuilder, 
            IBreadCrumbManager breadCrumbManager,
            IActionContextAccessor actionContextAccessor)
        {
            _pageTitleBuilder = pageTitleBuilder;
            _breadCrumbManager = breadCrumbManager;
            _actionContextAccessor = actionContextAccessor;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            var items = _breadCrumbManager.BuildMenu(ViewContext);


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

