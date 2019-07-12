using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Plato.Internal.Layout.ActionFilters;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Users.ActionFilters
{
    public class InvalidateUserFilter : IModularActionFilter
    {
        
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly SignInManager<User> _signInManager;

        private readonly IdentityOptions _identityOptions;

        public InvalidateUserFilter(
            IPlatoUserStore<User> platoUserStore,
            IOptions<IdentityOptions> options,
            SignInManager<User> signInManager)
        {
            _platoUserStore = platoUserStore;
            _signInManager = signInManager;
            _identityOptions = options.Value;
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

            // Not a view result
            if (!(context.Result is ViewResult))
            {
                return;
            }

            // Get IIdentity
            var identity = context.HttpContext.User?.Identity;
            if (identity == null)
            {
                return;
            }

            // Invalidate authentication if username no longer exists
            var check1 = await InvalidateUsernameAsync(identity);

            // Invalidate authentication if security stamp has changed
            var check2 = await InvalidateSecurityStampAsync(context.HttpContext);

            // If the user was invalidated redirect to root
            if (check1 || check2)
            {
                context.Result = new RedirectResult("/");
            }

        }

        public Task OnActionExecutedAsync(ResultExecutingContext context)
        {
            return Task.CompletedTask;
        }

        // ----------------------

        async Task<bool> InvalidateUsernameAsync(IIdentity identity)
        {
            
            // Not authenticated
            if (!identity.IsAuthenticated)
            {
                return false;
            }

            // No identity value
            if (string.IsNullOrEmpty(identity.Name))
            {
                return false;
            }

            // Attempt to find the user
            var user = await _platoUserStore.GetByUserNameAsync(identity.Name);
            if (user == null)
            {
                // User no longer exists or name has changed
                await _signInManager.SignOutAsync();
                // Indicate sign out
                return true;
            }

            return false;

        }

        async Task<bool> InvalidateSecurityStampAsync(HttpContext context)
        {

            // Get user from context
            var user = context.Features[typeof(User)] as User;

            // We are not authenticated
            if (user == null)
            {
                return false;
            }
            
            // Get security stamp claim
            var claim =
                context.User?.Claims?.FirstOrDefault(c =>
                    c.Type == _identityOptions.ClaimsIdentity.SecurityStampClaimType);

            // Ensure we found the claim
            if (claim == null)
            {
                return false;
            }

            // Ensure we have a security stamp
            if (string.IsNullOrEmpty(claim.Value))
            {
                return false;
            }

            // Sign out if the security stamps don't match
            if (!user.SecurityStamp.Equals(claim.Value, StringComparison.Ordinal))
            {
                // Security stamps no longer match, sign out the user
                await _signInManager.SignOutAsync();
                // Indicate sign out
                return true;
            }

            return false;

        }
        
    }

}
