using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Layout;
using Plato.Internal.Layout.ActionFilters;
using Plato.Internal.Security.Abstractions;

namespace Plato.Admin.ActionFilters
{
    public class AuthorizationFilter : IModularActionFilter
    {

        private readonly IAuthorizationService _authorizationService;

        public AuthorizationFilter(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
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

            // Check early to ensure we are working with a LayoutViewModel
            if (!(result?.Model is LayoutViewModel model))
            {
                return;
            }
            
            // If we are accessing an Admin controller check standard permissions

            var controllerName = context.RouteData.Values["controller"].ToString();
            switch (controllerName)
            {
                case "Admin":
                    if (!await _authorizationService.AuthorizeAsync(context.HttpContext.User, StandardPermissions.AdminAccess))
                    {
                        context.Result = new RedirectToRouteResult("UnauthorizedPage", new RouteValueDictionary()
                        {
                            ["area"] = "Plato.Core",
                            ["controller"] = "Home",
                            ["action"] = "Denied"
                        });
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
