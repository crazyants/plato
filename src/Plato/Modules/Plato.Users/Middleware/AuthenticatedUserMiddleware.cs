using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Models.Users;
using Plato.Internal.Shell.Abstractions;
using Plato.Users.Services;

namespace Plato.Users.Middleware
{

    public class AuthenticatedUserMiddleware
    {

        internal const string CookieName = "plato_active";

        private readonly RequestDelegate _next;

        public AuthenticatedUserMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            
            // Attempt to update last login date
            await UpdateAuthenticatedUsersLastLoginDateAsync(context);

            // Return next delegate
            await _next(context);

        }

        async Task UpdateAuthenticatedUsersLastLoginDateAsync(HttpContext context)
        {

            var cookie = context.Request.Cookies[CookieName];
            if (cookie != null)
            {
                return;
            }

            var contextFacade = context.RequestServices.GetRequiredService<IContextFacade>();
            if (contextFacade == null)
            {
                return;
            }

            // Is the request authenticated?
            var user = await contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return;
            }

            user.LastLoginDate = DateTimeOffset.UtcNow;

            var userManager = context.RequestServices.GetRequiredService<IPlatoUserManager<User>>();
            if (userManager == null)
            {
                return;
            }
            
            var managerResult = await userManager.UpdateAsync(user);
            if (managerResult.Succeeded)
            {
                // Set cookie to prevent further execution
                var tennantPath = "/";
                var shellSettings = context.RequestServices.GetRequiredService<IShellSettings>();
                if (shellSettings != null)
                {
                    tennantPath += shellSettings.RequestedUrlPrefix;
                }

                context.Response.Cookies.Append(
                    CookieName,
                    true.ToString(),
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Path = tennantPath,
                        Expires = DateTime.Now.AddMinutes(20)
                    });

            }

        }

    }

}
