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
using Plato.Navigation;

namespace Plato.Layout.TagHelpers
{

    [HtmlTargetElement("pager")]
    public class PagerTagHelper : TagHelper
    {

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

            _routeData = new RouteValueDictionary(_actionContextAccesor.ActionContext.RouteData.Values);

            output.TagName = "nav";
            output.Attributes.Add("aria-label", "Page navigation");
            
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(await Build());
        }

        private async Task<IHtmlContent> Build()
        {

            var builder = new HtmlContentBuilder();
            BuildHtml(builder, "<ul class=\"pagination\">");
            BuildFirst(builder);
            BuildPrevious(builder);
            BuildLinks(builder);
            BuildNext(builder);
            BuildLast(builder);
            BuildHtml(builder, "</ul>");

            return await Task.FromResult(builder);

        }

        HtmlContentBuilder BuildHtml(HtmlContentBuilder builder, string html)
        {
            builder.AppendHtml(html);
            return builder;
        }
        
        HtmlContentBuilder BuildFirst(HtmlContentBuilder builder)
        {

            var text = FirstText ?? T["First"];
            builder
                .AppendHtml("<li class=\"page-item\">")
                .AppendHtml("<a class=\"page-link\" href=\"#\" aria-label=\"Previous\" >")
                .AppendHtml("<span aria-hidden=\"true\">")
                .AppendHtml(text)
                .AppendHtml("</span>")
                .AppendHtml("</a>")
                .AppendHtml("</li>");

            return builder;

        }

        HtmlContentBuilder BuildPrevious(HtmlContentBuilder builder)
        {
            var text = PreviousText ?? T["&laquo;"];
            builder
                .AppendHtml("<li class=\"page-item\">")
                .AppendHtml("<a class=\"page-link\" href=\"#\" aria-label=\"Previous\" >")
                .AppendHtml("<span aria-hidden=\"true\">")
                .AppendHtml(text)
                .AppendHtml("</span>")
                .AppendHtml("</a>")
                .AppendHtml("</li>");

            return builder;
        }
        
        HtmlContentBuilder BuildLinks(HtmlContentBuilder builder)
        {

            if (Model == null)
                throw new ArgumentNullException(nameof(Model));
            
            if (Model.PageSize == 0)
                throw new ArgumentNullException(nameof(Model.PageSize));

            var currentPage = Model.Page;
            if (currentPage < 1)
                currentPage = 1;
            var pageSize = (int)Model.PageSize;
            var total = (double)Model.Total;;
            var numberOfPagesToShow = 7;
            var totalPageCount = Model.PageSize > 0 ? (int) Math.Ceiling(total / pageSize) : 1;
            var firstPage = Math.Max(1, Model.Page - (numberOfPagesToShow / 2));
            var lastPage = Math.Min(totalPageCount, Model.Page + (int) (numberOfPagesToShow / 2));

            if (numberOfPagesToShow > 0 && lastPage > 1)
            {
                for (var i = firstPage; i <= lastPage; i++)
                {

                    if (i == 1)
                        _routeData.Remove(pageKey);
                    else
                        _routeData[pageKey] = i;

                    var url = _urlHelper.RouteUrl(new UrlRouteContext {Values = _routeData});

                    builder
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

            return builder;

        }

        HtmlContentBuilder BuildNext(HtmlContentBuilder builder)
        {

            var text = NextText ?? T["&raquo;"];
            builder
                .AppendHtml("<li class=\"page-item\">")
                .AppendHtml("<a class=\"page-link\" href=\"#\" aria-label=\"Next\" >")
                .AppendHtml("<span aria-hidden=\"true\">")
                .AppendHtml(text)
                .AppendHtml("</span>")
                .AppendHtml("</a>")
                .AppendHtml("</li>");

            return builder;

        }

        HtmlContentBuilder BuildLast(HtmlContentBuilder builder)
        {
            var text = LastText ?? T["Last"];
            builder
                .AppendHtml("<li class=\"page-item\">")
                .AppendHtml("<a class=\"page-link\" href=\"#\" aria-label=\"Previous\" >")
                .AppendHtml("<span aria-hidden=\"true\">")
                .AppendHtml(text)
                .AppendHtml("</span>")
                .AppendHtml("</a>")
                .AppendHtml("</li>");
            return builder;
        }



    }
}
