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
    
    [HtmlTargetElement(Attributes = "asp-authorize,asp-permission")]
    [HtmlTargetElement(Attributes = "asp-permission,asp-resource")]
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

            // Get available permissions
            var permissions = _permissionManager.GetPermissions();

            // Find permission
            var permission = permissions?.FirstOrDefault(p => p.Name.Equals(Permission, StringComparison.OrdinalIgnoreCase));

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

            // Authorization failed - Suppress output
            if (!result.Succeeded)
            {
                output.SuppressOutput();
            }

        }

    }
}
