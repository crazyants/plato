using System;
using System.Collections.Generic;
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
            
            var items = _navigationManager.BuildMenu("Admin", _actionContextAccesor.ActionContext);

            foreach (var item in items)
            {
                
            }

            output.TagName = "a";     
            
            // Replaces <email> with <a> tag
            var content = await output.GetChildContentAsync();
            var target = content.GetContent() + "@" + EmailDomain;
            output.Attributes.SetAttribute("href", "mailto:" + target);
            output.Content.SetContent(target);

        }
    }
}
