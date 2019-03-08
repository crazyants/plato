using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Entities.Labels.Assets;
using Plato.Entities.Labels.Handlers;
using Plato.Entities.Labels.Models;
using Plato.Entities.Labels.Repositories;
using Plato.Entities.Labels.Services;
using Plato.Entities.Labels.Stores;
using Plato.Entities.Labels.Subscribers;

namespace Plato.Entities.Labels
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
            
            // Feature event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Repositories
            services.AddScoped<ILabelRepository<LabelBase>, LabelRepository<LabelBase>>();
            services.AddScoped<ILabelDataRepository<LabelData>, LabelDataRepository>();
            services.AddScoped<ILabelRoleRepository<LabelRole>, LabelRoleRepository>();
            services.AddScoped<IEntityLabelRepository<EntityLabel>, EntityLabelRepository>();

            // Stores
            services.AddScoped<ILabelStore<LabelBase>, LabelStore<LabelBase>>();
            services.AddScoped<ILabelDataStore<LabelData>, LabelDataStore>();
            services.AddScoped<ILabelRoleStore<LabelRole>, LabelRoleStore>();
            services.AddScoped<IEntityLabelStore<EntityLabel>, EntityLabelStore>();

            // Managers
            services.AddScoped<ILabelManager<LabelBase>, LabelManager<LabelBase>>();
            services.AddScoped<IEntityLabelManager<EntityLabel>, EntityLabelManager>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, EntityLabelSubscriber>();
          
            // Client assets
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Label service
            services.AddScoped<ILabelService<LabelBase>, LabelService<LabelBase>>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Api routes
            routes.MapAreaRoute(
                name: "EntityLabelsWebApi",
                areaName: "Plato.Entities.Labels",
                template: "api/{controller}/{action}/{id?}",
                defaults: new { controller = "Labels", action = "Get" }
            );


        }
    }
}