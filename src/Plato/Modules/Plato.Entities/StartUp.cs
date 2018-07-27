using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Abstractions.SetUp;
using Plato.Entities.Handlers;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Shell;

namespace Plato.Entities
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

            // Set-up event handler
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // Feature event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Repositories
            services.AddScoped<IEntityRepository<Entity>, EntityRepository<Entity>>();
            services.AddScoped<IEntityDataRepository<IEntityData>, EntityDataRepository>();
            services.AddScoped<IEntityReplyRepository<EntityReply>, EntityReplyRepository<EntityReply>>();
            
            // Stores
            services.AddScoped<IEntityStore<Entity>, EntityStore<Entity>>();
            services.AddScoped<IEntityDataStore<IEntityData>, EntityDataStore>();
            services.AddScoped<IEntityDataItemStore<EntityData>, EntityDataItemStore<EntityData>>();
            services.AddScoped<IEntityReplyStore<EntityReply>, EntityReplyStore<EntityReply>>();

            // Managers
            services.AddScoped<IEntityManager<Entity>, EntityManager<Entity>>();
            services.AddScoped<IEntityReplyManager<EntityReply>, EntityReplyManager<EntityReply>>();

            //services.AddScoped<IEntityDetailsStore<EntityDetails>, EntityDetailsStore>();


        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}