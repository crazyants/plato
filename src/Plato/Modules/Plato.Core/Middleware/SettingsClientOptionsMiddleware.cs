using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Scripting.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Core.Middleware
{

    public class SettingsClientOptionsMiddleware
    {
  
        private readonly RequestDelegate _next;

        public SettingsClientOptionsMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {

            // Register client options for site settings with our script manager
            var scriptManager = context.RequestServices.GetRequiredService<IScriptManager>();
            var scriptBlock = await BuildScriptBlock(context);
            if (scriptBlock != null)
            {
                scriptManager?.RegisterScriptBlock(scriptBlock, ScriptSection.Footer);
            }
            
            await _next(context);

        }
        
        async Task<ScriptBlock> BuildScriptBlock(HttpContext context)
        {
            
            // Settings store
            var siteSettingsStore = context.RequestServices.GetRequiredService<ISiteSettingsStore>();
            if (siteSettingsStore == null)
            {
                return null;
            }

            // Get settings
            var siteSettings = await siteSettingsStore.GetAsync();
            if (siteSettings == null)
            {
                return null;
            }
            
            // Register core client options by extending $.Plato.Options
            // i.e. $.extend($.Plato.Options, newOptions);
            var script = "$(function (win) { $.extend(win.$.Plato.Options, { locale: '{locale}' }); } (window));";
            script = script.Replace("{locale}", siteSettings.Culture);
            return new ScriptBlock(script, int.MinValue);

        }
        
    }

}
