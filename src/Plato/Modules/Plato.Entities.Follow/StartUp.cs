using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.Follow.ViewProviders;
using Plato.Entities.Models;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Entities.Follow.Handlers;
using Plato.Entities.Follow.Models;
using Plato.Entities.Follow.Repositories;
using Plato.Entities.Follow.Assets;
using Plato.Entities.Follow.Stores;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Entities.Follow
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

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();
            
            // View providers
            services.AddScoped<IViewProviderManager<Entity>, ViewProviderManager<Entity>>();
            services.AddScoped<IViewProvider<Entity>, FollowViewProvider>();
         
            // Register client assets
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Data access
            services.AddScoped<IEntityFollowRepository<EntityFollow>, EntityFollowRepository>();
            services.AddScoped<IEntityFollowStore<EntityFollow>, EntityFollowStore>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            routes.MapAreaRoute(
                name: "EntitiesFollowWebApi",
                areaName: "Plato.Entities.Follow",
                template: "api/follows/{controller}/{action}/{id?}",
                defaults: new { controller = "Follow", action = "Get" }
            );

        }
    }
}