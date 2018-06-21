using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace Plato.Internal.Layout.TagHelpers
{

    [HtmlTargetElement("userphoto")]
    public class UserPhotoTagHelper : TagHelper
    {

        public int UserId { get; set; }


        private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccesor;

        public UserPhotoTagHelper(
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

            // Ensure no surrounding element
            output.TagName = "img";
            output.TagMode = TagMode.SelfClosing;
            output.Attributes.Add("src", url);

            return Task.CompletedTask;

        }


    }
}
