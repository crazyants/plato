using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Features.Updates.Handlers;
using Plato.Features.Updates.Navigation;
using Plato.Features.Updates.ViewModels;
using Plato.Features.Updates.ViewProviders;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Features.Updates
{
    public class Startup : StartupBase
    {
    
        public override void ConfigureServices(IServiceCollection services)
        {
            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // View providers
            services.AddScoped<IViewProviderManager<FeatureUpdatesIndexViewModel>, ViewProviderManager<FeatureUpdatesIndexViewModel>>();
            services.AddScoped<IViewProvider<FeatureUpdatesIndexViewModel>, AdminViewProvider>();
       
            // Register moderation permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Index
            routes.MapAreaRoute(
                name: "FeatureUpdatesIndex",
                areaName: "Plato.Features.Updates",
                template: "admin/features/updates",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}