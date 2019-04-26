using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Admin.Models;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Reports.Assets;
using Plato.Reports.Models;
using Plato.Reports.Navigation;
using Plato.Reports.Services;
using Plato.Reports.ViewProviders;

namespace Plato.Reports
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

            // Register navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Register report index view providers
            services.AddScoped<IViewProviderManager<ReportIndex>, ViewProviderManager<ReportIndex>>();
            services.AddScoped<IViewProvider<ReportIndex>, AdminViewProvider>();
        
            // Register report page views view providers
            services.AddScoped<IViewProviderManager<PageViewsReport>, ViewProviderManager<PageViewsReport>>();
            services.AddScoped<IViewProvider<PageViewsReport>, PageViewsViewProvider>();

            // Register admin index View providers
            services.AddScoped<IViewProviderManager<AdminIndex>, ViewProviderManager<AdminIndex>>();
            services.AddScoped<IViewProvider<AdminIndex>, AdminIndexViewProvider>();

            // Services
            services.AddScoped<IDateRangeStorage, RouteValueDateRangeStorage>();

            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            // Index
            routes.MapAreaRoute(
                name: "ReportsIndex",
                areaName: "Plato.Reports",
                template: "admin/reports",
                defaults: new { controller = "Admin", action = "Index" }
            );
            
            // Page Views
            routes.MapAreaRoute(
                name: "ReportsPageViews",
                areaName: "Plato.Reports",
                template: "admin/reports/page-views/{pager.offset:int?}",
                defaults: new { controller = "Admin", action = "PageViews" }
            );


        }
    }
}