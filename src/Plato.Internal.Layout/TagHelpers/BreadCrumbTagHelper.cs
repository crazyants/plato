using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Plato.Internal.Navigation;

namespace Plato.Internal.Layout.TagHelpers
{


    [HtmlTargetElement("breadcrumb")]
    public class BreadCrumbTagHelper : TagHelper
    {
        private object _cssClasses;


        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IActionContextAccessor _actionContextAccesor;

        public BreadCrumbTagHelper(
            IActionContextAccessor actionContextAccesor,
            IBreadCrumbManager breadCrumbManager)
        {
            _actionContextAccesor = actionContextAccesor;
            _breadCrumbManager = breadCrumbManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            // Get default CSS classes
            _cssClasses = output.Attributes.FirstOrDefault(a => a.Name == "class")?.Value;
            
            output.TagName = "nav";
            output.Attributes.Add("aria-label", "breadcrumb");
            output.TagMode = TagMode.StartTagAndEndTag;
            
            var items = _breadCrumbManager.BuildMenu(_actionContextAccesor.ActionContext);
            
            output.PreContent.SetHtmlContent(BuildBreadCrumb(items));

        }


        private string BuildBreadCrumb(IEnumerable<MenuItem> items)
        {

            var sb = new StringBuilder();
            sb.Append("<ol class=\"breadcrumb\">");

            if (items != null)
            {
                foreach (var item in items)
                {
                    sb.Append("<li  class=\"breadcrumb-item\">")
                        .Append(item.Text)
                        .Append("</li>");
                }
            }


            sb.Append("</ol>");

            return sb.ToString();

        }

    }
}
