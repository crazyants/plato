using System;
using System.Threading.Tasks;
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
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {

            // Register client options for web api with our script manager
            var scriptBlock = BuildScriptBlock(context);
            if (scriptBlock != null)
            {
                var scriptManager = context.RequestServices.GetRequiredService<IScriptManager>();
                scriptManager?.RegisterScriptBlock(scriptBlock, ScriptSection.Footer);
            }
            
            await _next(context);

        }
        
        ScriptBlock BuildScriptBlock(HttpContext context)
        {
            
            var webApiOptions = context.RequestServices.GetRequiredService<IOptions<WebApiOptions>>();
            if (webApiOptions == null)
            {
                return null;
            }

            // Register client options for $.Plato.Http by extending $.Plato.Options
            // i.e. $.extend($.Plato.Options, newOptions);
            var script = "$(function (win) { $.extend(win.$.Plato.Options, { url: '{url}', apiKey: '{apiKey}', csrfCookieName: '{csrfCookieName}' }); } (window));";
            script = script.Replace("{url}", webApiOptions.Value.Url);
            script = script.Replace("{apiKey}", webApiOptions.Value.ApiKey);
            script = script.Replace("{csrfCookieName}", PlatoAntiForgeryOptions.AjaxCsrfTokenCookieName);

            return new ScriptBlock(script, int.MinValue);

        }
        
    }

}
