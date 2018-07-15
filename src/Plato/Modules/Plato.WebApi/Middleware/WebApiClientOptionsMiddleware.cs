using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Hosting.Abstractions;
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
            scriptManager?.RegisterScriptBlock(new ScriptBlock(await BuildScriptBlock(context)), ScriptSection.Footer);
            
            await _next(context);

        }
        
        async Task<string> BuildScriptBlock(HttpContext context)
        {

            var contextFacade = context.RequestServices.GetRequiredService<IContextFacade>();

            // Register client script to set-up $.Plato.Http
            var script = "$(function (win) { win.PlatoOptions = { url: '{url}', apiKey: '{apiKey}' } } (window));";
            script = script.Replace("{url}", await contextFacade.GetBaseUrl());
            script = script.Replace("{apiKey}", await GetApiKey(contextFacade));

            return script;

        }

        async Task<string> GetApiKey(IContextFacade contextFacade)
        {

            var settings = await contextFacade.GetSiteSettingsAsync();

            if (settings == null)
            {
                return string.Empty;
            }

            var user = await contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return settings.ApiKey;
            }

            if (String.IsNullOrWhiteSpace(user.ApiKey))
            {
                return settings.ApiKey;
            }

            return $"{settings.ApiKey}:{user.ApiKey}";

        }




    }


}
