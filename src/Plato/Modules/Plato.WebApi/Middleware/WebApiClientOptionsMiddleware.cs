using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Internal.Scripting.Abstractions;

namespace Plato.WebApi.Middleware
{
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
            
            var options = context.RequestServices.GetRequiredService<IOptions<WebApiOptions>>();
            
            // Register client script to set-up $.Plato.Http
            var script = "$(function (win) { win.PlatoOptions = { url: '{url}', apiKey: '{apiKey}', xsrfToken: '{xsrfToken}' } } (window));";
            script = script.Replace("{url}", options.Value.Url);
            script = script.Replace("{apiKey}", options.Value.ApiKey);
            script = script.Replace("{xsrfToken}", GetAntiforgeryToken(context));
            
            return new ScriptBlock(script, int.MinValue);

        }

        string GetAntiforgeryToken(HttpContext context)
        {
            var antiforgery = context.RequestServices.GetService<IAntiforgery>();
            var tokens = antiforgery.GetAndStoreTokens(context);
            return tokens.RequestToken;
        }
  
    }

}
