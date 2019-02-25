using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{

    [HtmlTargetElement("breadcrumb")]
    public class BreadCrumbTagHelper : TagHelper
    {

        private readonly HtmlEncoder _htmlEncoder;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IActionContextAccessor _actionContextAccesor;

        public BreadCrumbTagHelper(
            IActionContextAccessor actionContextAccesor,
            IBreadCrumbManager breadCrumbManager)
        {
            _actionContextAccesor = actionContextAccesor;
            _breadCrumbManager = breadCrumbManager;
            _htmlEncoder = HtmlEncoder.Default;
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            output.TagName = "ol";
            output.TagMode = TagMode.StartTagAndEndTag;
            
            var items = _breadCrumbManager
                .BuildMenu(_actionContextAccesor.ActionContext);

            if (items != null)
            {
                output.PreContent.SetHtmlContent(BuildBreadCrumb(items));
            }

            return Task.CompletedTask;

        }
        
        string BuildBreadCrumb(IEnumerable<MenuItem> items)
        {

            var sb = new StringBuilder();
            foreach (var item in items)
            {
                BuildBreadCrumbItem(item, sb);
            }

            return sb.ToString();

        }

        void BuildBreadCrumbItem(MenuItem item, StringBuilder sb)
        {
         
            var hasUrl = !String.IsNullOrEmpty(item.Href);

            sb.Append("<li  class=\"breadcrumb-item\">");

            if (hasUrl)
            {
                sb.Append("<a href=\"").Append(item.Href).Append("\">");
            }
            
            // Ensure item.Text is always encoded
            // This can contain user supplied input
            sb.Append(_htmlEncoder.Encode(item.Text));

            if (hasUrl)
            {
                sb.Append("</a>");
            }

            sb.Append("</li>");

        }

    }

}
