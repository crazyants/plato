using System;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Plato.WebApi.Middleware;

namespace Plato.WebApi.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateClientAntiForgeryTokenAttribute : Attribute, IAuthorizationFilter
    {
        
        public void OnAuthorization(AuthorizationFilterContext context)
        {

            // Get CSRF options
            var antiforgeryOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<AntiforgeryOptions>>();
            
            // Ensure anti forgery options have been configured
            if (antiforgeryOptions == null)
            {
                throw new ArgumentNullException(nameof(antiforgeryOptions));
            }

            // Ensure we have a antiforgery header
            var headers = context.HttpContext.Request.Headers;
            if (!headers.ContainsKey(antiforgeryOptions.Value.HeaderName))
            {
                context.Result = new ForbidResult();
                return;
            }

            var headerValue = string.Empty;
            if (headers[antiforgeryOptions.Value.HeaderName].Count > 0)
            {
                headerValue = headers[antiforgeryOptions.Value.HeaderName][0];
            }

            if (String.IsNullOrWhiteSpace(headerValue))
            {
                context.Result = new ForbidResult();
                return;
            }
            
            var cookie = context.HttpContext.Request.Cookies[PlatoAntiForgeryOptions.AjaxCsrfTokenCookieName];
            if (cookie == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            // Our "X-Csrf-Token" header does not match current antiforgery cookie
            if (!cookie.Equals(headerValue, StringComparison.Ordinal))
            {
                context.Result = new ForbidResult();
                return;
            }

            return;

        }
    }

}