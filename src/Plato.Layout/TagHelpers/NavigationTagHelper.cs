using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Navigation;

namespace Plato.Layout.TagHelpers
{
    
    [HtmlTargetElement("navigation")]
    public class NavigationTagHelper : TagHelper
    {

        public string Name { get; set; } = "Site";

        public string EmailDomain { get; set; } = "instantasp.co.uk";
        
        private readonly INavigationManager _navigationManager;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccesor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly string NewLine = Environment.NewLine;
        int _level = 0;

        public NavigationTagHelper(
            INavigationManager navigationManager,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccesor,
            IHttpContextAccessor httpContextAccessor)
        {
            _navigationManager = navigationManager;
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccesor = actionContextAccesor;
            _httpContextAccessor = httpContextAccessor;
        }

        private object _cssClasses; 

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            // Get default CSS classes
            _cssClasses = output.Attributes.FirstOrDefault(a => a.Name == "class")?.Value;

            // Ensure no surrounding element
            output.TagName = "";
            output.TagMode = TagMode.StartTagAndEndTag;

            // Build navigation
            var sb = new StringBuilder();
            var items = _navigationManager.BuildMenu(
                this.Name,
                _actionContextAccesor.ActionContext);
            if (items != null)
            {
                BuildNavigationRecursivly(items.ToList(), sb);
            }
            
            output.PreContent.SetHtmlContent(sb.ToString());
           

        }

        string BuildNavigationRecursivly(
            List<MenuItem> items, 
            StringBuilder sb)
        {
          
            var ulClass = $"{_cssClasses}";
            if (_level > 0)
                ulClass = "dropdown-menu";
            
            sb.Append(NewLine);
            AddTabs(_level, sb);
            
            sb.Append("<ul class=\"")
                .Append(ulClass)
                .Append("\">")
                .Append(NewLine);

            var index = 0;
            foreach (var item in items)
            {
                
                AddTabs(_level + 1, sb);

                sb.Append("<li class=\"")
                    .Append(GetListItemClass(items, item, index))
                    .Append("\">");
                
                var linkClass = _level == 0 
                    ? "nav-link" 
                    : "dropdown-item";

                if (item.Items.Count > 0)
                {
                    if (!string.IsNullOrEmpty(linkClass))
                        linkClass += " ";
                    linkClass += "dropdown-toggle";
                    foreach (var className in item.Classes)
                    {
                        if (!string.IsNullOrEmpty(linkClass))
                            linkClass += " ";
                        linkClass += className;
                    }
                }
                
                sb.Append("<a class=\"")
                    .Append(linkClass)
                    .Append("\" href=\"")
                    .Append(item.Href)
                    .Append("\"")
                    .Append(item.Items.Count > 0 ? " data-toggle=\"dropdown\"" : "")
                    .Append(">")
                    .Append(item.Text.Value)
                    .Append("</a>");
                
                if (item.Items.Count > 0)
                {
                    _level++;
                    BuildNavigationRecursivly(item.Items, sb);
                    AddTabs(_level, sb);
                    _level--;
                }

                sb.Append("</li>")
                    .Append(NewLine);

                index += 1;
            }

            AddTabs(_level, sb);
            sb.Append("</ul>")
                .Append(NewLine);
            
            return sb.ToString();
            
        }

        string GetListItemClass(
            ICollection items,
            MenuItem item,
            int index)
        {

            var sb = new StringBuilder();
            sb.Append("nav-item");

            if (item.Items.Count > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" ");
                sb.Append("dropdown");
            }

            if (_level > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" ");
                sb.Append("dropdown-submenu");
            }
            
            if (index == 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" ");
                sb.Append("first");
            }

            if (index == items.Count - 1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" ");
                sb.Append("last");
            }

            return sb.ToString();

        }

        StringBuilder AddTabs(int level, StringBuilder sb)
        {
            for (var i = 0; i < level; i++)
            {
                sb
                    .Append("   ");
            }

            return sb;
        }

    }
}
