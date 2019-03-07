using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Models.Users;
using Plato.Internal.Reputations.Abstractions;
using Plato.Users.Services;

namespace Plato.Users.Middleware
{

    public class AuthenticatedUserMiddleware
    {
        static readonly object SyncLock = new object();
        internal const string CookieName = "plato_active";
        private readonly RequestDelegate _next;

        public AuthenticatedUserMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            
            // Sign out the request if the user is not found
            await SignOutRequestIfUserNotFound(context);

            // Hydrate HttpContext.Features with our user
            await HydrateHttpContextFeature(context);

            // Attempt to update last login date
            await UpdateAuthenticatedUsersLastLoginDateAsync(context);
            
            // Return next delegate
            await _next.Invoke(context);

        }

        #region "Private Methods"

        async Task SignOutRequestIfUserNotFound(HttpContext context)
        {

            // If the request is not authenticated move along
            if (!context.User.Identity.IsAuthenticated)
            {
                return;
            }

            // Get context facade
            var contextFacade = context.RequestServices.GetRequiredService<IContextFacade>();
            if (contextFacade == null)
            {
                return;
            }

            // Attempt to get the user
            var user = await contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                // If the request is authenticated but we didn't find a user attempt to sign out the request
                var signInManager = context.RequestServices.GetRequiredService<SignInManager<User>>();
                if (signInManager != null)
                {
                    await signInManager.SignOutAsync();
                }
            }
            
        }
        
        async Task HydrateHttpContextFeature(HttpContext context)
        {

            // If the request is not authenticated move along
            if (!context.User.Identity.IsAuthenticated)
            {
                return;
            }

            // Get context facade
            var contextFacade = context.RequestServices.GetRequiredService<IContextFacade>();
            if (contextFacade == null)
            {
                return;
            }

            // Attempt tto get user from data store
            var user = await contextFacade.GetAuthenticatedUserAsync();

            // User not found
            if (user == null)
            {
                return;
            }

            lock (SyncLock)
            {
                // Add authenticated user to features for subsequent use
                context.Features[typeof(User)] = user;
            }
            
        }
        
        async Task UpdateAuthenticatedUsersLastLoginDateAsync(HttpContext context)
        {

            // If the request is not authenticated move along
            if (!context.User.Identity.IsAuthenticated)
            {
                return;
            }
            
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

            user.Visits += 1;
            user.VisitsUpdatedDate = DateTimeOffset.UtcNow;
            user.LastLoginDate = DateTimeOffset.UtcNow;

            var userManager = context.RequestServices.GetRequiredService<IPlatoUserManager<User>>();
            if (userManager == null)
            {
                return;
            }
            
            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {

                // Award visit reputation
                var userReputationAwarder = context.RequestServices.GetRequiredService<IUserReputationAwarder>();
                if (userReputationAwarder != null)
                {
                    await userReputationAwarder.AwardAsync(
                        Reputations.UniqueVisit,
                        result.Response.Id);
                }
                
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

        #endregion

    }

}
