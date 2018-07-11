using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Shell;

namespace Plato.Categories
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

            //// Set-up event handler
            //services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            //// Feature event handler
            //services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            //// Repositories
            //services.AddScoped<IEntityRepository<Entity>, EntityRepository>();
            //services.AddScoped<IEntityDataRepository<EntityData>, EntityDataRepository>();
            //services.AddScoped<IEntityReplyRepository<EntityReply>, EntityReplyRepository>();
            
            //// Stores
            //services.AddScoped<IEntityStore<Entity>, EntityStore>();
            //services.AddScoped<IEntityDataStore<EntityData>, EntityDataStore>();
            //services.AddScoped<IEntityDataItemStore<EntityData>, EntityDataItemStore<EntityData>>();
            //services.AddScoped<IEntityReplyStore<EntityReply>, EntityReplyStore>();

            //// Managers
            //services.AddScoped<IEntityManager<Entity>, EntityManager>();
            //services.AddScoped<IEntityReplyManager<EntityReply>, EntityReplyManager>();


        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}