using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Navigation;

namespace Plato.Internal.Layout.TagHelpers
{

    [HtmlTargetElement("pager")]
    public class PagerTagHelper : TagHelper
    {

        private const int NumberOfPagesToShow = 7;
        private int _totalPageCount;

        public PagerOptions Model { get; set; }

        public IStringLocalizer T { get; set; }

        public string FirstText { get; set; }

        public string PreviousText { get; set; }

        public string NextText { get; set; }

        public string LastText { get; set; }

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }
        
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccesor;
        private readonly IUrlHelper _urlHelper;
        private string pageKey = "page";

        private RouteValueDictionary _routeData;

        public PagerTagHelper(
            IStringLocalizer<PagerTagHelper> localizer,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccesor,
            IUrlHelperFactory urlHelperFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccesor = actionContextAccesor;
            _urlHelper = urlHelperFactory.GetUrlHelper(_actionContextAccesor.ActionContext);
            T = localizer;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
         
            // Calculate total pages
            _totalPageCount = Model.PageSize > 0 ? (int)Math.Ceiling((double)Model.Total / Model.PageSize) : 1;
            
            // Get route data
            _routeData = new RouteValueDictionary(_actionContextAccesor.ActionContext.RouteData.Values);

            // Define taghelper
            output.TagName = "nav";
            output.Attributes.Add("aria-label", "Page navigation");
            
            output.TagMode = TagMode.StartTagAndEndTag;

            // No need to display if we only have a single page
            if (_totalPageCount > 1)
            {
                output.Content.SetHtmlContent(await Build());
            }
            
        }

        async Task<IHtmlContent> Build()
        {
            
            var builder = new HtmlContentBuilder();
            var htmlContentBuilder = builder
                .AppendHtml("<ul class=\"pagination\">")
                .AppendHtml(BuildFirst())
                .AppendHtml(BuildPrevious())
                .AppendHtml(BuildLinks())
                .AppendHtml(BuildNext())
                .AppendHtml(BuildLast())
                .AppendHtml("</ul>");

            return await Task.FromResult(htmlContentBuilder);

        }
        
        HtmlString BuildFirst()
        {

            var text = FirstText ?? T["First"];

            var builder = new HtmlContentBuilder();
            var htmlContentBuilder = builder
                .AppendHtml("<li class=\"page-item\">")
                .AppendHtml("<a class=\"page-link\" href=\"#\" aria-label=\"Previous\" >")
                .AppendHtml("<span aria-hidden=\"true\">")
                .AppendHtml(text)
                .AppendHtml("</span>")
                .AppendHtml("</a>")
                .AppendHtml("</li>");

            return htmlContentBuilder.ToHtmlString();

        }

        HtmlString BuildPrevious()
        {
            var text = PreviousText ?? T["&laquo;"];

            var builder = new HtmlContentBuilder();
            var htmlContentBuilder = builder
                .AppendHtml("<li class=\"page-item\">")
                .AppendHtml("<a class=\"page-link\" href=\"#\" aria-label=\"Previous\" >")
                .AppendHtml("<span aria-hidden=\"true\">")
                .AppendHtml(text)
                .AppendHtml("</span>")
                .AppendHtml("</a>")
                .AppendHtml("</li>");

            return htmlContentBuilder.ToHtmlString();
        }

        HtmlString BuildLinks()
        {

            if (Model == null)
                throw new ArgumentNullException(nameof(Model));
            
            if (Model.PageSize == 0)
                throw new ArgumentNullException(nameof(Model.PageSize));

            var currentPage = Model.Page;
            if (currentPage < 1)
                currentPage = 1;
            
            var firstPage = Math.Max(1, Model.Page - (NumberOfPagesToShow / 2));
            var lastPage = Math.Min(_totalPageCount, Model.Page + (int) (NumberOfPagesToShow / 2));
            
            IHtmlContentBuilder output = null;

            if (NumberOfPagesToShow > 0 && lastPage > 1)
            {

                output = new HtmlContentBuilder();

                for (var i = firstPage; i <= lastPage; i++)
                {

                    if (i == 1)
                        _routeData.Remove(pageKey);
                    else
                        _routeData[pageKey] = i;

                    var url = _urlHelper.RouteUrl(new UrlRouteContext {Values = _routeData});

                    var builder = new HtmlContentBuilder();
                    output = builder
                        .AppendHtml("<li class=\"page-item")
                        .AppendHtml(i == Model.Page ? " active" : "")
                        .AppendHtml("\">")
                        .AppendHtml("<a class=\"page-link\" href=\"")
                        .AppendHtml(url)
                        .AppendHtml("\">")
                        .AppendHtml(i.ToString())
                        .AppendHtml("</a>")
                        .AppendHtml("</li>");
                }
            }

            if (output != null)
            {
                return output.ToHtmlString();
            }

            return HtmlString.Empty;

        }

        HtmlString BuildNext()
        {

            var text = NextText ?? T["&raquo;"];

            var builder = new HtmlContentBuilder();
            var htmlContentBuilder = builder
                .AppendHtml("<li class=\"page-item\">")
                .AppendHtml("<a class=\"page-link\" href=\"#\" aria-label=\"Next\" >")
                .AppendHtml("<span aria-hidden=\"true\">")
                .AppendHtml(text)
                .AppendHtml("</span>")
                .AppendHtml("</a>")
                .AppendHtml("</li>");

            return htmlContentBuilder.ToHtmlString();

        }

        HtmlString BuildLast()
        {
            var text = LastText ?? T["Last"];

            var builder = new HtmlContentBuilder();
            var htmlContentBuilder = builder
                .AppendHtml("<li class=\"page-item\">")
                .AppendHtml("<a class=\"page-link\" href=\"#\" aria-label=\"Previous\" >")
                .AppendHtml("<span aria-hidden=\"true\">")
                .AppendHtml(text)
                .AppendHtml("</span>")
                .AppendHtml("</a>")
                .AppendHtml("</li>");

            return htmlContentBuilder.ToHtmlString();
        }
        
    }

}
