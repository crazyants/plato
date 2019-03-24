using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Security.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{
    
    [HtmlTargetElement(Attributes = "asp-permission")]
    [HtmlTargetElement(Attributes = "asp-permission,asp-resource")]
    [RestrictChildren("authorize-success", "authorize-fail")]
    public class AuthorizeTagHelper : TagHelper
    {

        private readonly IPermissionsManager<Permission> _permissionManager;
        private readonly IAuthorizationService _authorizationService;
        
        [HtmlAttributeName("asp-permission")]
        public string Permission { get; set; }

        [HtmlAttributeName("asp-resource")]
        public object Resource { get; set; }
        
        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }
        
        public AuthorizeTagHelper(
            IAuthorizationService authorizationService,
            IPermissionsManager<Permission> permissionManager)
        {
            _authorizationService = authorizationService;
            _permissionManager = permissionManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            // Create a context for our success and failure tag helpers
            var authorizeContext = new AuthorizeContext();
            context.Items.Add(typeof(AuthorizeContext), authorizeContext);

            // Required to invoke child tag helpers
            var content = await output.GetChildContentAsync();
            
            // Find permission
            var permission = _permissionManager
                .GetPermissions()?
                .FirstOrDefault(p => p.Name.Equals(Permission, StringComparison.OrdinalIgnoreCase));

            // We always need a permission
            if (permission == null)
            {
                return;
            }
            
            // Validate against registered permission handlers
            var result = await _authorizationService.AuthorizeAsync(
                ViewContext.HttpContext.User,
                Resource,
                new PermissionRequirement(permission));

            // Authorization failed 
            if (result.Succeeded)
            {
                if (authorizeContext.Success != null)
                {
                    var success = new TagBuilder("div");
                    if (!string.IsNullOrEmpty(authorizeContext.Success.CssClass))
                    {
                        success.AddCssClass(authorizeContext.Success.CssClass);
                    }

                    success.InnerHtml.AppendHtml(authorizeContext.Success.Content);
                    output.Content.AppendHtml(success);
                }
            }
            else
            {
                if (authorizeContext.Fail != null)
                {
                    var fail = new TagBuilder("div");
                    if (!string.IsNullOrEmpty(authorizeContext.Fail.CssClass))
                    {
                        fail.AddCssClass(authorizeContext.Fail.CssClass);
                    }
                    fail.InnerHtml.AppendHtml(authorizeContext.Fail.Content);
                    output.Content.AppendHtml(fail);
                }
                else
                {
                    // Suppress output if authorization failed and we have no failure content
                    //output.SuppressOutput();
                }
            }

        }

    }
}
