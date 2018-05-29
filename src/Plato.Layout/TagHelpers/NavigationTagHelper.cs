using System;
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

        public string UlClass { get; set; }

        public string EmailDomain { get; set; } = "instantasp.co.uk";


        private readonly INavigationManager _navigationManager;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccesor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        private static string _newLine = Environment.NewLine;

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

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            var sb = new StringBuilder();
            var items = _navigationManager.BuildMenu(this.Name, _actionContextAccesor.ActionContext);
            if (items != null)
            {
                BuildNavigationRecursivly(items.ToList(), sb);
            }

            output.PreContent.SetHtmlContent(sb.ToString());


            //// Replaces <email> with <a> tag
            //var content = await output.GetChildContentAsync();
            //var target = content.GetContent() + "@" + EmailDomain;
            //output.Attributes.SetAttribute("href", "mailto:" + target);
            //output.Content.SetContent(target);

        }

        private int _level = 0;

        private string BuildNavigationRecursivly(
            List<MenuItem> items, 
            StringBuilder sb)
        {

            var ulClass = UlClass;
         
            if (_level > 0)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    ulClass += " ";
                ulClass = "dropdown-menu";
            }
            
            sb.Append(_newLine);
            AddTabs(_level, sb);
            
            sb.Append("<ul class=\"")
                .Append(ulClass)
                .Append("\">")
                .Append(_newLine);

            var index = 0;
            foreach (var item in items)
            {

                var liClass = GetListItemClass(items, item, index);

                AddTabs(_level + 1, sb);

                sb.Append("<li class=\"")
                    .Append(liClass)
                    .Append("\">");


                var linkClass = _level == 0 
                    ? "nav-link" 
                    : "dropdown-item";

                if (item.Items.Count > 0)
                {
                    if (!string.IsNullOrEmpty(linkClass))
                        linkClass += " ";
                    linkClass += "dropdown-toggle";
                }
                
                sb.Append("<a class=\"")
                    .Append(linkClass)
                    .Append("\" href=\"")
                    .Append(item.Href)
                    .Append("\" data-toggle=\"dropdown\">")
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
                    .Append(_newLine);

                index += 1;
            }

            AddTabs(_level, sb);
            sb.Append("</ul>")
                .Append(_newLine);
            
            return sb.ToString();
            
        }

        private string GetListItemClass(
            List<MenuItem> items,
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

        public StringBuilder AddTabs(int level, StringBuilder sb)
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
