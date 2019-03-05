using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.History.Assets;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Entities.History.Handlers;
using Plato.Entities.History.Models;
using Plato.Entities.History.Repositories;
using Plato.Entities.History.Services;
using Plato.Entities.History.Stores;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Entities.History
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

            // Data access
            services.AddScoped<IEntityHistoryRepository<EntityHistory>, EntityHistoryRepository>();
            services.AddScoped<IEntityHistoryStore<EntityHistory>, EntityHistoryStore>();

            // Managers
            services.AddScoped<IEntityHistoryManager<EntityHistory>, EntityHistoryManager>();
            
            // Register assets
            services.AddScoped<IAssetProvider, AssetProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "EntitiesHistoryWebApi",
                areaName: "Plato.Entities.History",
                template: "api/history/{controller}/{action}/{id?}",
                defaults: new { controller = "Entity", action = "Get" }
            );

        }
    }
}