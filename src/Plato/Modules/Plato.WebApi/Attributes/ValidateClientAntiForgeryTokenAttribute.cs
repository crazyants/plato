using System;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.WebApi.Middleware;

namespace Plato.WebApi.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateClientAntiForgeryTokenAttribute : Attribute, IAuthorizationFilter
    {
        
        public void OnAuthorization(AuthorizationFilterContext context)
        {

            // Get CSRF options
            var antiForgeryOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<AntiforgeryOptions>>();
            
            // Ensure anti forgery options have been configured
            if (antiForgeryOptions == null)
            {
                throw new ArgumentNullException(nameof(antiForgeryOptions));
            }

            // Ensure we have a anti forgery header
            var headers = context.HttpContext.Request.Headers;
            if (!headers.ContainsKey(antiForgeryOptions.Value.HeaderName))
            {
                context.Result = new ForbidResult();
                return;
            }

            var headerValue = string.Empty;
            if (headers[antiForgeryOptions.Value.HeaderName].Count > 0)
            {
                headerValue = headers[antiForgeryOptions.Value.HeaderName][0];
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

            // Our "X-Csrf-Token" header does not match current anti forgery cookie
            if (!cookie.Equals(headerValue, StringComparison.Ordinal))
            {
                context.Result = new ForbidResult();
                return;
            }

            return;

        }
    }

}