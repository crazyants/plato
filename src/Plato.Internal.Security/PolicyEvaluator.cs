using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Security.Abstractions;

namespace Plato.Internal.Security
{

    //public class PolicyEvaluator : IPolicyEvaluator
    //{

    //    private readonly IAuthorizationService _authorizationService;

    //    // TODO: Simple implementation. We probably don't need this
    //    public PolicyEvaluator(IAuthorizationService authorizationService)
    //    {
    //        _authorizationService = authorizationService;
    //    }

    //    public async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
    //    {

    //        var permission = new Permission();
    //        var result = await _authorizationService.AuthorizeAsync(context.User, policy);
    //        if (result.Succeeded)
    //        {
    //            return AuthenticateResult.Success(null);

    //        }
            
    //        return AuthenticateResult.Fail(new Exception($"Authentication failed for user '{context.User.Identity.Name}'."));

    //    }

    //    public async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context,
    //        object resource)
    //    {


    //        var permission = new Permission();
            
    //        var result = await _authorizationService.AuthorizeAsync(context.User, resource, policy);
    //        if (result.Succeeded)
    //        {
    //            return PolicyAuthorizationResult.Success();
    //        }
    //        return PolicyAuthorizationResult.Forbid();

    //    }

    //}

}
