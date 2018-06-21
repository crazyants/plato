using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace Plato.Internal.Layout.TagHelpers
{

    [HtmlTargetElement("avatar")]
    public class AvatarTagHelper : TagHelper
    {

        public int UserId { get; set; }
        
        private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccesor;

        public AvatarTagHelper(
            IUrlHelperFactory urlHelperFactory,
            IHttpContextAccessor httpContextAccessor, 
            IActionContextAccessor actionContextAccesor)
        {
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccesor = actionContextAccesor;
            _urlHelper = urlHelperFactory.GetUrlHelper(_actionContextAccesor.ActionContext);
        }


        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            var url = _urlHelper.RouteUrl(new UrlRouteContext
            {
                Values = new RouteValueDictionary()
                {
                    {"Area", "Plato.Users"},
                    {"Controller", "Photo"},
                    {"Action", "Serve"},
                    {"Id", this.UserId}
                }
            });
            
            output.TagName = "div";
            output.TagMode = TagMode.SelfClosing;
            output.Attributes.Add("style", $"background-image: url('{url}');");
            
            return Task.CompletedTask;

        }


    }
}
