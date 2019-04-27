using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Features.Handlers;
using Plato.Features.Navigation;
using Plato.Features.ViewModels;
using Plato.Features.ViewProviders;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Features
{
    public class Startup : StartupBase
    {
    
        public override void ConfigureServices(IServiceCollection services)
        {
            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Setup event handler
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();
            
            // View providers
            services.AddScoped<IViewProviderManager<FeaturesIndexViewModel>, ViewProviderManager<FeaturesIndexViewModel>>();
            services.AddScoped<IViewProvider<FeaturesIndexViewModel>, AdminViewProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            //routes.MapAreaRoute(
            //    name: "AdminFeatures",
            //    areaName: "Plato.Features",
            //    template: "admin/features/{action}/{id?}",
            //    defaults: new { controller = "Admin", action = "Index" }
            //);

            //routes.MapAreaRoute(
            //    name: "AdminEnableFeatures",
            //    areaName: "Plato.Features",
            //    template: "admin/features/{action}/{id?}",
            //    defaults: new { controller = "Admin", action = "Enable" }
            //);

        }
    }
}