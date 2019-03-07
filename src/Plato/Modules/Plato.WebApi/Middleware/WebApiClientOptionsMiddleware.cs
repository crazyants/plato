using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Scripting.Abstractions;
using Plato.WebApi.Services;

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
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {

            // Register client options for web api with our script manager
            var scriptBlock = await BuildScriptBlock(context);
            if (scriptBlock != null)
            {
                var scriptManager = context.RequestServices.GetRequiredService<IScriptManager>();
                scriptManager?.RegisterScriptBlock(scriptBlock, ScriptSection.Footer);
            }
            
            await _next.Invoke(context);

        }
        
        async Task<ScriptBlock> BuildScriptBlock(HttpContext context)
        {

            var webApiOptionsFactory = context.RequestServices.GetRequiredService<IWebApiOptionsFactory>();
            if (webApiOptionsFactory == null)
            {
                return null;
            }

            var settings = await webApiOptionsFactory.GetSettingsAsync();

            // Register client options for $.Plato.Http by extending $.Plato.defaults
            // i.e. $.extend($.Plato.defaults, newOptions);
            var script = "$(function (win) { $.extend(win.$.Plato.defaults, { url: '{url}', apiKey: '{apiKey}', csrfCookieName: '{csrfCookieName}' }); } (window));";
            script = script.Replace("{url}", settings.Url);
            script = script.Replace("{apiKey}", settings.ApiKey);
            script = script.Replace("{csrfCookieName}", PlatoAntiForgeryOptions.AjaxCsrfTokenCookieName);

            return new ScriptBlock(script, int.MinValue);

        }
        
    }

}
