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
using Plato.Internal.Security.Abstractions;

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
                name: "FeatureCategoryIndex",
                areaName: "Plato.Features",
                template: "admin/features/category/{opts.category?}",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }
    }
}