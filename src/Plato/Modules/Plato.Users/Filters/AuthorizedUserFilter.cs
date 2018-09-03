using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Plato.Internal.Models.Shell;
using Plato.Internal.Models.Users;
using Plato.Users.Services;

namespace Plato.Users.Filters
{
    public class AuthorizedUserFilter : IActionFilter, IAsyncResultFilter
    {

        readonly string _tenantPath;
        internal const string CookieName = "plato_active";
        
        private readonly ILogger<AuthorizedUserFilter> _logger;
        private readonly IPlatoUserManager<User> _platoUserManager;

        public AuthorizedUserFilter(
            IShellSettings shellSettings,
            ILogger<AuthorizedUserFilter> logger,
            IPlatoUserManager<User> platoUserManager)
        {
            _logger = logger;
            _platoUserManager = platoUserManager;
            _tenantPath = "/" + shellSettings.RequestedUrlPrefix;
        }
        
        #region "Implementation"

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {


        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {

            // Ensure code only executes when cookie does not exist
            var cookie = context.HttpContext.Request.Cookies[CookieName];

            // Cookie exists no need to continue
            if (cookie != null)
            {
                return;
            }

            // Prevent code from executing for cookie lifetime
            context.HttpContext.Response.Cookies.Append(
                CookieName,
                true.ToString(),
                new CookieOptions
                {
                    HttpOnly = true,
                    Path = _tenantPath,
                    Expires = DateTime.Now.AddMinutes(20)
                });

            // Is the request authenticated?
            var result = await _platoUserManager.GetAuthenticatedUserAsync(context.HttpContext.User);

            // No need to continue if the request is not authenciated
            if (!result.Succeeded)
            {
                return;
            }
            
            // Update last login date for authenticated user
            result.Response.LastLoginDate = DateTimeOffset.UtcNow;
            
            // Persist last login date update
            await _platoUserManager.UpdateAsync(result.Response);
            
        }

        #endregion

    }
}
