using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ActionFilters;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Models.Shell;
using Plato.Internal.Models.Users;
using Plato.Internal.Reputations.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Users.ActionFilters
{
    public class UpdateUserLastLoginDateFilter : IModularActionFilter
    {

        internal const string CookieName = "plato_active";
        private bool _active = false;
        readonly string _tenantPath;
        
        private readonly IUserReputationAwarder _userReputationAwarder;
        private readonly IPlatoUserStore<User> _userStore;
        private readonly IContextFacade _contextFacade;

        public UpdateUserLastLoginDateFilter(
            IUserReputationAwarder userReputationAwarder,
            IShellSettings shellSettings,
            IContextFacade contextFacade,
            IPlatoUserStore<User> userStore)
        {
            _tenantPath = "/" + shellSettings.RequestedUrlPrefix;
            _userReputationAwarder = userReputationAwarder;
            _contextFacade = contextFacade;
            _userStore = userStore;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

            // Get tracking cookie
            var value = Convert.ToString(context.HttpContext.Request.Cookies[CookieName]);

            // Cookie does not exist
            if (String.IsNullOrEmpty(value))
            {
                _active = false;
                return;
            }

            // We have an active cookie, ensure this is known for the entire request
            _active = true;

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

            // Tracking cookie already exists, simply execute the controller result
            if (_active)
            {
                return;
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Not authenticated, simply execute the controller result
            if (user == null)
            {
                return;
            }

            user.Visits += 1;
            user.VisitsUpdatedDate = DateTimeOffset.UtcNow;
            user.LastLoginDate = DateTimeOffset.UtcNow;

            var result = await _userStore.UpdateAsync(user);
            if (result != null)
            {

                // Award visit reputation
                await _userReputationAwarder.AwardAsync(new Reputation("Visit", 1), result.Id, "Unique Visit");

                // Set client cookie to ensure update does not
                // occur again for as long as the cookie exists
                context.HttpContext.Response.Cookies.Append(
                    CookieName,
                    true.ToString(),
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Path = _tenantPath,
                        Expires = DateTime.Now.AddMinutes(20)
                    });

            }

        }

        public Task OnActionExecutedAsync(ResultExecutingContext context)
        {
            return Task.CompletedTask;
        }
    }

}
