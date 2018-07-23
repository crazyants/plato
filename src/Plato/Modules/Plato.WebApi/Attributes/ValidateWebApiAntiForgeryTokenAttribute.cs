using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Plato.WebApi.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateWebApiAntiForgeryTokenAttribute : Attribute, IAuthorizationFilter
    {

        private const string HeaderName = "X-Xsrf-Token";

        
        public void OnAuthorization(AuthorizationFilterContext context)
        {

            var antiforgeryOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<AntiforgeryOptions>>();


            // Ensure anti forgery options have been configured
            if (antiforgeryOptions == null)
            {
                throw new ArgumentNullException(nameof(antiforgeryOptions));
            }

            // Ensure we have a antiforgery header
            var headers = context.HttpContext.Request.Headers;
            if (!headers.ContainsKey(HeaderName))
            {
                context.Result = new ForbidResult();
            }
            
            var headerValue = string.Empty;
            if (headers[HeaderName].Count > 0)
            {
                headerValue = headers[HeaderName][0];
            }

            if (String.IsNullOrWhiteSpace(headerValue))
            {
                context.Result = new ForbidResult();
            }
            
            var cookieName = antiforgeryOptions.Value.Cookie.Name;
            
            // No name is configured
            if (String.IsNullOrWhiteSpace(cookieName))
            {
                context.Result = new ForbidResult();
            }
            
            var cookie = context.HttpContext.Request.Cookies[cookieName];
            if (cookie == null)
            {
                context.Result = new ForbidResult();
            }

            // Our "X-Xsrf-Token" header does not match current antiforgery cookie
            if (cookie != headerValue)
            {
                context.Result = new ForbidResult();
            }

        }
    }

}
