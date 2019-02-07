using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Abstractions.SetUp;
using Plato.Core.Handlers;
using Plato.Core.Middleware;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;

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

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Register client options middleware 
            app.UseMiddleware<SettingsClientOptionsMiddleware>();
            
            // Error page route
            routes.MapAreaRoute(
                name: "ErrorPage",
                areaName: "Plato.Core",
                template: "error",
                defaults: new { controller = "Home", action = "Error" }
            );

            // Unauthorized / access denied page route
            routes.MapAreaRoute(
                name: "UnauthorizedPage",
                areaName: "Plato.Core",
                template: "denied",
                defaults: new { controller = "Home", action = "Denied" }
            );

        }
    }
}