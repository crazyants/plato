using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Layout.TagHelpers
{

    [HtmlTargetElement("avatar")]
    public class AvatarTagHelper : TagHelper
    {

        private const string AreaAttributeName = "Area";
        private const string ControllerAttributeName = "Controller";
        private const string ActionAttributeName = "Action";
        private const string IdAttributeName = "Id";

        public const string Area = "Plato.Users";
        public const string Controller = "Photo";
        public const string Action = "Serve";
        
        public int UserId { get; set; }
        
        public ISimpleUser User { get; set; } 

        private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AvatarTagHelper(
            IUrlHelperFactory urlHelperFactory,
            IHttpContextAccessor httpContextAccessor, 
            IActionContextAccessor actionContextAccesor)
        {
            _httpContextAccessor = httpContextAccessor;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccesor.ActionContext);
        }
        
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            var url = _urlHelper.RouteUrl(new UrlRouteContext
            {
                Values = new RouteValueDictionary()
                {
                    {AreaAttributeName, Area},
                    {ControllerAttributeName, Controller},
                    {ActionAttributeName, Action},
                    {IdAttributeName, this.UserId}
                }
            });
            
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
  
            var img = new TagBuilder("span");
            img.Attributes.Add("style", $"background-image: url('{url}');");
            output.Content.SetHtmlContent(img.ToHtmlString());

            return Task.CompletedTask;

        }


    }
}
