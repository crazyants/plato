using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Abstractions.SetUp;
using Plato.Core.Handlers;
using Plato.Core.Middleware;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Core
{
    public class Startup : StartupBase
    {
        private readonly IShellSettings _shellSettings;
        private readonly string _tenantPrefix;
        private readonly string _cookieSuffix;

        public Startup(IShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
            _tenantPrefix = shellSettings.RequestedUrlPrefix;
            _cookieSuffix = shellSettings.AuthCookieName;
        }

        public override void ConfigureServices(IServiceCollection services)
        {

            // Set-up event handler
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();
            
            // Configure current culture
            services.Configure<LocaleOptions>(options =>
            {
                var contextFacade = services.BuildServiceProvider().GetService<IContextFacade>();
                options.WatchForChanges = false;
                options.Culture = contextFacade.GetCurrentCultureAsync().Result;
            });

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Register client options middleware 
            app.UseMiddleware<SettingsClientOptionsMiddleware>();
            
       
            // Unauthorized - 401
            routes.MapAreaRoute(
                name: "UnauthorizedPage",
                areaName: "Plato.Core",
                template: "denied",
                defaults: new { controller = "Home", action = "Denied" }
            );

            // Moved - 404
            routes.MapAreaRoute(
                name: "MovedPage",
                areaName: "Plato.Core",
                template: "moved",
                defaults: new { controller = "Home", action = "Moved" }
            );

            // Error page - 500
            routes.MapAreaRoute(
                name: "ErrorPage",
                areaName: "Plato.Core",
                template: "error",
                defaults: new { controller = "Home", action = "Error" }
            );
            
        }

    }

}