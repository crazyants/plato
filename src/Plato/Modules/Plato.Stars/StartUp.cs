using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Stars.Assets;
using Plato.Stars.Handlers;
using Plato.Stars.Models;
using Plato.Stars.Repositories;
using Plato.Stars.Services;
using Plato.Stars.Stores;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Stars
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
            services.AddScoped<IStarRepository<Star>, StarRepository>();
            services.AddScoped<IStarStore<Star>, StarStore>();
            
            // Follow Type Manager
            services.AddScoped<IStarTypesManager, StarTypesManager>();
            
            // Follow type providers
            services.AddScoped<IStarTypeProvider, DefaultStarTypes>();

            // Follow manager
            services.AddScoped<IStarManager<Star>, StarManager>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            routes.MapAreaRoute(
                name: "StarsWebApi",
                areaName: "Plato.Stars",
                template: "api/stars/{controller}/{action}/{id?}",
                defaults: new { controller = "Entity", action = "Get" }
            );

        }
    }
}