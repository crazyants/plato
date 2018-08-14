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
        public static readonly string AjaxCsrfTokenCookieName = "plato_csrf_client";

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
            
            await _next(context);

        }
        
        ScriptBlock BuildScriptBlock(HttpContext context)
        {
            
            var webApiOptions = context.RequestServices.GetRequiredService<IOptions<WebApiOptions>>();
      
            // Register client options for $.Plato.Http
            var script = "$(function (win) { win.PlatoOptions = { url: '{url}', apiKey: '{apiKey}', csrfCookieName: '{csrfCookieName}' } } (window));";
            script = script.Replace("{url}", webApiOptions.Value.Url);
            script = script.Replace("{apiKey}", webApiOptions.Value.ApiKey);
            script = script.Replace("{csrfCookieName}", PlatoAntiForgeryOptions.AjaxCsrfTokenCookieName);

            return new ScriptBlock(script, int.MinValue);

        }
        
    }

}
