using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.Assets;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Entities.Handlers;
using Plato.Entities.Models;
using Plato.Entities.Navigation;
using Plato.Entities.Repositories;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.Subscribers;
using Plato.Entities.ViewProviders;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Search.Abstractions;

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
            
            // Feature event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Register client resources
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Register navigation provider
            services.AddScoped<INavigationProvider, UserSubmissionsMenu>();

            // Repositories
            services.AddScoped<IEntityRepository<Entity>, EntityRepository<Entity>>();
            services.AddScoped<IEntityDataRepository<IEntityData>, EntityDataRepository>();
            services.AddScoped<IEntityReplyRepository<EntityReply>, EntityReplyRepository<EntityReply>>();
            services.AddScoped<IEntityUsersRepository, EntityUsersRepository>();

            // Stores
            services.AddScoped<IEntityStore<Entity>, EntityStore<Entity>>();
            services.AddScoped<IEntityDataStore<IEntityData>, EntityDataStore>();
            services.AddScoped<IEntityDataItemStore<EntityData>, EntityDataItemStore<EntityData>>();
            services.AddScoped<IEntityReplyStore<EntityReply>, EntityReplyStore<EntityReply>>();
            services.AddScoped<IEntityUsersStore, EntityUsersStore>();

            // Managers
            services.AddScoped<IEntityManager<Entity>, EntityManager<Entity>>();
            services.AddScoped<IEntityReplyManager<EntityReply>, EntityReplyManager<EntityReply>>();

            // Services
            services.AddScoped<IEntityService<Entity>, EntityService<Entity>>();

            // Entity subscribers
            services.AddScoped<IBrokerSubscriber, ParseEntityAliasSubscriber>();
            services.AddScoped<IBrokerSubscriber, ParseEntityUrlsSubscriber>();
            services.AddScoped<IBrokerSubscriber, ParseEntityHtmlSubscriber>();

            // Full text index providers
            services.AddScoped<IFullTextIndexProvider, FullTextIndexes>();
        
            // Add profile views
            services.AddScoped<IViewProviderManager<Profile>, ViewProviderManager<Profile>>();
            services.AddScoped<IViewProvider<Profile>, ProfileViewProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "EntitiesWebApi",
                areaName: "Plato.Entities",
                template: "api/entities/{controller}/{action}/{id?}",
                defaults: new { controller = "Entity", action = "Get" }
            );

        }
    }
}