using System.Threading.Tasks;
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
            scriptManager?.RegisterScriptBlock(new ScriptBlock(await BuildScriptBlock(context), int.MinValue), ScriptSection.Footer);
            await _next(context);
        }
        
        Task<string> BuildScriptBlock(HttpContext context)
        {
            
            var options = context.RequestServices.GetRequiredService<IOptions<WebApiOptions>>();
    
            // Register client script to set-up $.Plato.Http
            var script = "$(function (win) { win.PlatoOptions = { url: '{url}', apiKey: '{apiKey}' } } (window));";
            script = script.Replace("{url}", options.Value.Url);
            script = script.Replace("{apiKey}", options.Value.ApiKey);

            return Task.FromResult(script);

        }
  
    }

}
