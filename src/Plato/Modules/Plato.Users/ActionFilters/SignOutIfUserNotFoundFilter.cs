using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ActionFilters;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Users.ActionFilters
{
    public class SignOutIfUserNotFoundFilter : IModularActionFilter
    {

        private readonly IContextFacade _contextFacade;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly SignInManager<User> _signInManager;

        public SignOutIfUserNotFoundFilter(
            IContextFacade contextFacade,
            IPlatoUserStore<User> platoUserStore,
            SignInManager<User> signInManager)
        {
            _contextFacade = contextFacade;
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

            // Attempt to find the user
            User user = null;
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

            if (!String.IsNullOrEmpty(identity.Name))
            {
                user = await _platoUserStore.GetByUserNameAsync(identity.Name);
            }

            // If the request is authenticated but we didn't find a user attempt to sign out the request
            if (user == null)
            {
                await _signInManager.SignOutAsync();
            }
            
        }

        public Task OnActionExecutedAsync(ResultExecutingContext context)
        {
            return Task.CompletedTask;
        }
    }
}
