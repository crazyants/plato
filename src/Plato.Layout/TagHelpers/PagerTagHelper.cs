using System;
using System.Text;
using System.Threading.Tasks;
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

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccesor;
        private readonly IUrlHelperFactory _urlHelperFactory;
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
            _urlHelperFactory = urlHelperFactory;
            _urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccesor.ActionContext);
            T = localizer;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "nav";
            output.Attributes.Add("aria-label", "Page navigation");

            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(await Build());
        }

        private async Task<string> Build()
        {
            _routeData = new RouteValueDictionary(_actionContextAccesor.ActionContext.RouteData.Values);
            
            var builder = new StringBuilder();
            builder.Append("<ul class=\"pagination\">");
            BuildFirst(builder);
            BuildPrevious(builder);
            BuildLinks(builder);
            BuildNext(builder);
            BuildLast(builder);
            builder.Append("</ul>");

            return await Task.FromResult(builder.ToString());

        }


        private StringBuilder BuildFirst(StringBuilder builder)
        {

            var text = FirstText ?? T["First"];
            builder
                .Append("<li class=\"page-item\">")
                .Append("<a class=\"page-link\" href=\"#\" aria-label=\"Previous\" >")
                .Append("<span aria-hidden=\"true\">")
                .Append(text)
                .Append("</span>")
                .Append("</a>")
                .Append("</li>");

            return builder;

        }

        private StringBuilder BuildPrevious(StringBuilder builder)
        {
            var text = PreviousText ?? T["&laquo;"];
            builder
                .Append("<li class=\"page-item\">")
                .Append("<a class=\"page-link\" href=\"#\" aria-label=\"Previous\" >")
                .Append("<span aria-hidden=\"true\">")
                .Append(text)
                .Append("</span>")
                .Append("</a>")
                .Append("</li>");

            return builder;
        }
        
        private StringBuilder BuildLinks(StringBuilder builder)
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
                        .Append("<li class=\"page-item")
                        .Append(i == Model.Page ? " active" : "")
                        .Append("\">")
                        .Append("<a class=\"page-link\" href=\"")
                        .Append(url)
                        .Append("\">")
                        .Append(i)
                        .Append("</a>")
                        .Append("</li>");
                }
            }

            return builder;

        }

        private StringBuilder BuildNext(StringBuilder builder)
        {

            var text = NextText ?? T["&raquo;"];
            builder
                .Append("<li class=\"page-item\">")
                .Append("<a class=\"page-link\" href=\"#\" aria-label=\"Next\" >")
                .Append("<span aria-hidden=\"true\">")
                .Append(text)
                .Append("</span>")
                .Append("</a>")
                .Append("</li>");

            return builder;

        }

        private StringBuilder BuildLast(StringBuilder builder)
        {
            var text = LastText ?? T["Last"];
            builder
                .Append("<li class=\"page-item\">")
                .Append("<a class=\"page-link\" href=\"#\" aria-label=\"Previous\" >")
                .Append("<span aria-hidden=\"true\">")
                .Append(text)
                .Append("</span>")
                .Append("</a>")
                .Append("</li>");
            return builder;
        }



    }
}
