using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;

namespace Plato.Layout.TagHelpers
{



    [HtmlTargetElement("pager")]
    public class PagerTagHelper : TagHelper
    {
        
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; }

        public double TotalResults { get; set; }
        
        public IStringLocalizer T { get; set; }
        
        public string FirstText { get; set; }

        public string PreviousText { get; set; }

        public string NextText { get; set; }

        public string LastText { get; set; }
        
        public RouteData RouteData { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccesor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        private IUrlHelper _urlHelper;

        private string pageKey = "page";
      
        public PagerTagHelper(
            IStringLocalizer<PagerTagHelper> localizer,
            IHttpContextAccessor httpContextAccessor, 
            IActionContextAccessor actionContextAccesor,
            IUrlHelperFactory urlHelperFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccesor = actionContextAccesor;
            _urlHelperFactory = urlHelperFactory;
            T = localizer;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "nav";
            output.Attributes.Add("aria-label", "Page navigation");

            output.TagMode = TagMode.StartTagAndEndTag;
            output.PreContent.SetHtmlContent(BuildControl());
        }


        private string BuildControl()
        {

            TotalResults = 500;
            PageSize = 20;

            var currentPage = PageIndex;
            if (currentPage < 1)
                currentPage = 1;
            var numberOfPagesToShow = 7;
            var totalPageCount = PageSize > 0 ? (int)Math.Ceiling(TotalResults / PageSize) : 1;
            var firstPage = Math.Max(1, PageIndex - (numberOfPagesToShow / 2));
            var lastPage = Math.Min(totalPageCount, PageIndex + (int)(numberOfPagesToShow / 2));

            var routeData = new RouteValueDictionary(
                _actionContextAccesor.ActionContext.RouteData.Values);
            
            var firstText = FirstText ?? T["First"];
            var previousText = PreviousText ?? T["&laquo;"];
            var nextText = NextText ?? T["&raquo;"];
            var lastText = LastText ?? T["Last"];
            
            var sb = new StringBuilder();
            sb.Append("<ul class=\"pagination\">");

            // first
            sb
                .Append("<li class=\"page-item\">")
                .Append("<a class=\"page-link\" href=\"#\" aria-label=\"Previous\" >")
                .Append("<span aria-hidden=\"true\">")
                .Append(firstText)
                .Append("</span>")
                .Append("</a>")
                .Append("</li>");
            
            // previous
            sb
                .Append("<li class=\"page-item\">")
                .Append("<a class=\"page-link\" href=\"#\" aria-label=\"Previous\" >")
                .Append("<span aria-hidden=\"true\">")
                .Append(previousText)
                .Append("</span>")
                .Append("</a>")
                .Append("</li>");
            
            if (numberOfPagesToShow > 0 && lastPage > 1)
            {
                for (var i = firstPage; i <= lastPage; i++)
                {

                    if (i == 1)
                        routeData.Remove(pageKey);
                    else
                        routeData[pageKey] = i;


                    if (_urlHelper == null)
                    {
                        _urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccesor.ActionContext);
                    }

                    var url = _urlHelper.RouteUrl(new UrlRouteContext { Values = routeData });

                    sb
                        .Append("<li class=\"page-item")
                        .Append(i == PageIndex ? " active" : "")
                        .Append("\">")
                        .Append("<a class=\"page-link\" href=\"#\">")
                        .Append(i)
                        .Append("</a>")
                        .Append("</li>");
                }
            }


            // page X of X
                    for (var i = 1; i <= 5; i++)
            {
                
                sb
                    .Append("<li class=\"page-item")
                    .Append(i == PageIndex ? " active" : "")
                    .Append("\">")
                    .Append("<a class=\"page-link\" href=\"#\">")
                    .Append(i)
                    .Append("</a>")
                    .Append("</li>");
            }

            // next
            sb
                .Append("<li class=\"page-item\">")
                .Append("<a class=\"page-link\" href=\"#\" aria-label=\"Next\" >")
                .Append("<span aria-hidden=\"true\">")
                .Append(nextText)
                .Append("</span>")
                .Append("</a>")
                .Append("</li>");

            // last
            sb
                .Append("<li class=\"page-item\">")
                .Append("<a class=\"page-link\" href=\"#\" aria-label=\"Previous\" >")
                .Append("<span aria-hidden=\"true\">")
                .Append(lastText)
                .Append("</span>")
                .Append("</a>")
                .Append("</li>");


            sb.Append("</ul>");

            return sb.ToString();

        }

    }
    }
