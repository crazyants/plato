using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Plato.Internal.Layout.ActionFilters;
using Plato.Internal.Security.Abstractions;

namespace Plato.Admin.ActionFilters
{

    /// <summary>
    /// An action filter that checks to ensure we have the
    /// necessary permissions to access any "Admin" controllers.
    /// </summary>
    public class AuthorizationFilter : IModularActionFilter
    {

        private readonly string _redirectRouteName;
        private readonly RouteValueDictionary _redirectRouteValues;

        private readonly IAuthorizationService _authorizationService;

        public AuthorizationFilter(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;

            // Unauthorized redirect
            _redirectRouteName = "UnauthorizedPage";
            _redirectRouteValues = new RouteValueDictionary()
            {
                ["area"] = "Plato.Core",
                ["controller"] = "Home",
                ["action"] = "Denied"
            };

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            return;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }

        public async Task OnActionExecutingAsync(ResultExecutingContext context)
        {
            
            // The controller action didn't return a view result so no need to continue execution
            if (!(context.Result is ViewResult result))
            {
                return;
            }
            
            // We need route values to check
            if (context.RouteData?.Values == null)
            {
                return;
            }

            // We need a controller name to check
            if (!context.RouteData.Values.ContainsKey("controller"))
            {
                return;
            }

            // If we are accessing an Admin controller check standard permissions
            var controllerName = context.RouteData.Values["controller"].ToString();
            switch (controllerName)
            {
                case "Admin":

                    // If we are not authenticated redirect to denied route immediately
                    if (!context.HttpContext.User.Identity.IsAuthenticated)
                    {
                        context.Result = new RedirectToRouteResult(_redirectRouteName, _redirectRouteValues);
                        return; // No need to continue execution
                    }

                    // Else check our claims 
                    if (!await _authorizationService.AuthorizeAsync(context.HttpContext.User, StandardPermissions.AdminAccess))
                    {
                        context.Result = new RedirectToRouteResult(_redirectRouteName, _redirectRouteValues);
                    }

                    break;

            }

        }

        public Task OnActionExecutedAsync(ResultExecutingContext context)
        {
            return Task.CompletedTask;
        }

    }

}
