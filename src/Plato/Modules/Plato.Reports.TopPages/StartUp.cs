using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Reports.TopPages.Navigation;
using Plato.Reports.TopPages.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Reports.TopPages.Models;

namespace Plato.Reports.TopPages
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

            // Register navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Register report page views view providers
            services.AddScoped<IViewProviderManager<FeatureViewIndex>, ViewProviderManager<FeatureViewIndex>>();
            services.AddScoped<IViewProvider<FeatureViewIndex>, AdminViewProvider>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            routes.MapAreaRoute(
                name: "ReportsTopPages",
                areaName: "Plato.Reports.TopPages",
                template: "admin/reports/top-pages",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}