using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Follow.Assets;
using Plato.Follow.Handlers;
using Plato.Follow.Models;
using Plato.Follow.Repositories;
using Plato.Follow.Services;
using Plato.Follow.Stores;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Follow
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

            // Follow Types
            services.AddScoped<IFollowTypesManager, FollowTypesManager>();
            
            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();
       
            // Register client assets
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Data access
            services.AddScoped<IFollowRepository<Models.Follow>, FollowRepository>();
            services.AddScoped<IFollowStore<Models.Follow>, FollowStore>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            routes.MapAreaRoute(
                name: "EntityFollowsWebApi",
                areaName: "Plato.Follow",
                template: "api/follows/{controller}/{action}/{id?}",
                defaults: new { controller = "Entity", action = "Get" }
            );

        }
    }
}