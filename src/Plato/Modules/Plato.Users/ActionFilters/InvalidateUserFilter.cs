using System;
using System.Linq;
using System.Security.Claims;
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
            _identityOptions = options.Value;
            _platoUserStore = platoUserStore;
            _signInManager = signInManager;
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

            // Not authenticated
            if (!identity.IsAuthenticated)
            {
                return;
            }

            // No identity value
            if (string.IsNullOrEmpty(identity.Name))
            {
                return;
            }

            // Invalidate authentication if username no longer exists
            var invalidIdentityName = await InvalidateIdentityNameAsync(identity);

            // Invalidate authentication if security stamp has changed
            var invalidSecurityStamp = await InvalidateSecurityStampAsync(context.HttpContext);

            // If the user was invalidated redirect to root
            if (invalidIdentityName || invalidSecurityStamp)
            {
                context.Result = new RedirectResult("/");
            }

        }

        public Task OnActionExecutedAsync(ResultExecutingContext context)
        {
            return Task.CompletedTask;
        }

        // ----------------------
        
        async Task<bool> InvalidateIdentityNameAsync(IIdentity identity)
        {
            
            // Attempt to find the user by the identity.Name value
            var user = await _platoUserStore.GetByUserNameAsync(identity.Name);

            // User no longer exists or name has changed
            if (user == null)
            {
                // Sign out
                await _signInManager.SignOutAsync();
                // Indicate user was invalidated
                return true;
            }

            return false;

        }

        async Task<bool> InvalidateSecurityStampAsync(HttpContext context)
        {
            
            // Get security stamp claim, avoiding LINQ for perf reasons
            Claim claim = null;
            if (context.User?.Claims != null)
            {
                foreach (var c in context.User?.Claims)
                {
                    if (c.Type == _identityOptions.ClaimsIdentity.SecurityStampClaimType)
                    {
                        claim = c;
                        break;
                    }
                }
            }
             
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
            
            // Get authenticated user from context
            if (!(context.Features[typeof(User)] is User user))
            {
                return false;
            }

            // Compare stamps
            if (!user.SecurityStamp.Equals(claim.Value, StringComparison.Ordinal))
            {
                // Security stamps no longer match, sign out the user
                await _signInManager.SignOutAsync();
                // Indicate user was invalidated
                return true;
            }

            return false;

        }
        
    }

}
