using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Follows.Assets;
using Plato.Follows.Handlers;
using Plato.Follows.Models;
using Plato.Follows.Repositories;
using Plato.Follows.Services;
using Plato.Follows.Stores;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Data.Migrations.Abstractions;

namespace Plato.Follows
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

            // Client assets
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Data access
            services.AddScoped<IFollowRepository<Models.Follow>, FollowRepository>();
            services.AddScoped<IFollowStore<Models.Follow>, FollowStore>();
            
            // Follow Type Manager
            services.AddScoped<IFollowTypesManager, FollowTypesManager>();
            
            // Follow type providers
            services.AddScoped<IFollowTypeProvider, DefaultFollowTypes>();

            // Follow managers
            services.AddScoped<IFollowManager<Models.Follow>, FollowManager>();
         
            // Migrations
            services.AddSingleton<IMigrationProvider, Migrations>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            routes.MapAreaRoute(
                name: "EntityFollowsWebApi",
                areaName: "Plato.Follows",
                template: "api/follows/{controller}/{action}/{id?}",
                defaults: new { controller = "Entity", action = "Get" }
            );

        }
    }
}