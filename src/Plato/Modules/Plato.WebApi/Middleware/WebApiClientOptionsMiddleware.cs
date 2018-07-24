using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Internal.Scripting.Abstractions;

namespace Plato.WebApi.Middleware
{

    public static class PlatoAntiForgeryOptions
    {
        public static readonly string AjaxCsrfTokenCookieName = "plato_antiforgery_client";

    }

    public class WebApiClientOptionsMiddleware
    {

  
        private readonly RequestDelegate _next;

        public WebApiClientOptionsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            // Register client options for web api with our script manager
            var scriptManager = context.RequestServices.GetRequiredService<IScriptManager>();
            scriptManager?.RegisterScriptBlock(BuildScriptBlock(context), ScriptSection.Footer);

            // Add client side accessible cookie with our XSRF token, the value of this
            // cookie is sent with API requests 
            var antiforgeryOptions = context.RequestServices.GetRequiredService<IOptions<AntiforgeryOptions>>();
            var antiforgery = context.RequestServices.GetService<IAntiforgery>();
            var tokens = antiforgery.GetAndStoreTokens(context);
            context.Response.Cookies.Append(PlatoAntiForgeryOptions.AjaxCsrfTokenCookieName, tokens.RequestToken,
                new CookieOptions() { HttpOnly = false });
            
            await _next(context);

        }
        
        ScriptBlock BuildScriptBlock(HttpContext context)
        {
            
            var webApiOptions = context.RequestServices.GetRequiredService<IOptions<WebApiOptions>>();
            var antiforgeryOptions = context.RequestServices.GetRequiredService<IOptions<AntiforgeryOptions>>();

            // Register client script to set-up $.Plato.Http
            var script = "$(function (win) { win.PlatoOptions = { url: '{url}', apiKey: '{apiKey}', csrfCookieName: '{csrfCookieName}' } } (window));";
            script = script.Replace("{url}", webApiOptions.Value.Url);
            script = script.Replace("{apiKey}", webApiOptions.Value.ApiKey);
            script = script.Replace("{csrfCookieName}", PlatoAntiForgeryOptions.AjaxCsrfTokenCookieName);

            return new ScriptBlock(script, int.MinValue);

        }

        //string GetAntiforgeryToken(HttpContext context)
        //{
        //    var antiforgery = context.RequestServices.GetService<IAntiforgery>();
        //    var tokens = antiforgery.GetAndStoreTokens(context);
        //    return tokens.RequestToken;
        //}
  
    }

}
