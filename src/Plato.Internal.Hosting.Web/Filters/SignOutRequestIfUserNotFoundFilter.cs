using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Internal.Hosting.Web.Filters
{

    //public class SignOutRequestIfUserNotFoundFilter : IActionFilter, IAsyncResultFilter
    //{

    //    private readonly IContextFacade _contextFacade;
    //    private readonly IPlatoUserStore<User> _platoUserStore;
    //    private readonly SignInManager<User> _signInManager;

    //    public SignOutRequestIfUserNotFoundFilter(
    //        IContextFacade contextFacade, 
    //        IPlatoUserStore<User> platoUserStore,
    //        SignInManager<User> signInManager)
    //    {
    //        _contextFacade = contextFacade;
    //        _platoUserStore = platoUserStore;
    //        _signInManager = signInManager;
    //    }

    //    public void OnActionExecuting(ActionExecutingContext context)
    //    {
    //        return;
    //    }

    //    public void OnActionExecuted(ActionExecutedContext context)
    //    {

    //        return;
    //    }

    //    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    //    {

    //        // Not a view result
    //        if (!(context.Result is ViewResult))
    //        {
    //            await next();
    //            return;
    //        }

    //        // Attempt to find the user
    //        User user = null;
    //        var identity = context.HttpContext.User?.Identity;

    //        if (identity == null)
    //        {
    //            await next();
    //            return;
    //        }

    //        // Not authenticated
    //        if (!identity.IsAuthenticated)
    //        {
    //            await next();
    //            return;
    //        }

    //        if (!String.IsNullOrEmpty(identity.Name))
    //        {
    //            user = await _platoUserStore.GetByUserNameAsync(identity.Name);
    //        }
            
    //        // If the request is authenticated but we didn't find a user attempt to sign out the request
    //        if (user == null)
    //        {
    //            await _signInManager.SignOutAsync();
    //        }
            
    //        await next();

    //    }

    //}

}
