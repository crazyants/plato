using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Abstractions.Routing;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Site.Assets;

namespace Plato.Site
{
    public class Startup : StartupBase
    {
        private readonly IShellSettings _shellSettings;

        public Startup(IShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public override void ConfigureServices(IServiceCollection services)
        {

            // Register assets
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Homepage route providers
            services.AddSingleton<IHomeRouteProvider, HomeRoutes>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            
            // About
            routes.MapAreaRoute(
                name: "PlatoSiteAbout",
                areaName: "Plato.Site",
                template: "about",
                defaults: new { controller = "Home", action = "About" }
            );

            // Features
            routes.MapAreaRoute(
                name: "PlatoSiteFeatures",
                areaName: "Plato.Site",
                template: "features",
                defaults: new { controller = "Home", action = "Features" }
            );

            // Modules
            routes.MapAreaRoute(
                name: "PlatoSiteModules",
                areaName: "Plato.Site",
                template: "modules",
                defaults: new { controller = "Home", action = "Modules" }
            );

            // Pricing
            routes.MapAreaRoute(
                name: "PlatoSitePricing",
                areaName: "Plato.Site",
                template: "pricing",
                defaults: new { controller = "Home", action = "Pricing" }
            );

            // Support Options
            routes.MapAreaRoute(
                name: "PlatoSiteSupport",
                areaName: "Plato.Site",
                template: "support/options",
                defaults: new { controller = "Home", action = "SupportOptions" }
            );
            
            // Contact
            routes.MapAreaRoute(
                name: "PlatoSiteContact",
                areaName: "Plato.Site",
                template: "contact",
                defaults: new { controller = "Home", action = "Contact" }
            );

            // Privacy
            routes.MapAreaRoute(
                name: "PlatoSitePrivacy",
                areaName: "Plato.Site",
                template: "privacy",
                defaults: new { controller = "Home", action = "Privacy" }
            );

            // Terms
            routes.MapAreaRoute(
                name: "PlatoSiteTerms",
                areaName: "Plato.Site",
                template: "terms",
                defaults: new { controller = "Home", action = "Terms" }
            );

            // Catch All
            routes.MapAreaRoute(
                name: "PlatoSiteIndex",
                areaName: "Plato.Site",
                template: "site/{controller}/{action}",
                defaults: new { controller = "Home", action = "Index" }
            );

        }
    }
}