using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Reports.PageViews.Navigation;
using Plato.Reports.PageViews.Models;
using Plato.Internal.Layout.ViewProviders;
using Plato.Reports.PageViews.ViewProviders;

namespace Plato.Reports.PageViews
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
            services.AddScoped<IViewProviderManager<PageViewIndex>, ViewProviderManager<PageViewIndex>>();
            services.AddScoped<IViewProvider<PageViewIndex>, AdminViewProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            // Page Views
            routes.MapAreaRoute(
                name: "ReportsPageViews",
                areaName: "Plato.Reports.PageViews",
                template: "admin/reports/page-views/{pager.offset:int?}",
                defaults: new { controller = "Admin", action = "Index" }
            );
            
        }

    }

}