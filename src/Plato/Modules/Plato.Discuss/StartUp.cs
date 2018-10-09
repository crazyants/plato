using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Discuss.Handlers;
using Plato.Discuss.Assets;
using Plato.Discuss.Models;
using Plato.Discuss.Navigation;
using Plato.Discuss.Services;
using Plato.Discuss.Subscribers;
using Plato.Discuss.ViewProviders;
using Plato.Entities.Repositories;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.Subscribers;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Discuss
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

            // Register navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();
            services.AddScoped<INavigationProvider, ProfileMenu>();
            services.AddScoped<INavigationProvider, SearchMenu>();

            // Stores
            services.AddScoped<IEntityRepository<Topic>, EntityRepository<Topic>>();
            services.AddScoped<IEntityStore<Topic>, EntityStore<Topic>>();
            services.AddScoped<IEntityManager<Topic>, EntityManager<Topic>>();

            services.AddScoped<IEntityReplyRepository<Reply>, EntityReplyRepository<Reply>>();
            services.AddScoped<IEntityReplyStore<Reply>, EntityReplyStore<Reply>>();
            services.AddScoped<IEntityReplyManager<Reply>, EntityReplyManager<Reply>>();

            // Register data access
            services.AddScoped<IPostManager<Topic>, TopicManager>();
            services.AddScoped<IPostManager<Reply>, ReplyManager>();

            // Services
            services.AddScoped<ITopicService, TopicService>();
            
            // Register client resources
            services.AddScoped<IAssetProvider, AssetProvider>();
            
            // Register view providers
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, TopicViewProvider>();
            services.AddScoped<IViewProviderManager<Reply>, ViewProviderManager<Reply>>();
            services.AddScoped<IViewProvider<Reply>, ReplyViewProvider>();

            // Add discussion views to base user pages
            services.AddScoped<IViewProviderManager<UserProfile>, ViewProviderManager<UserProfile>>();
            services.AddScoped<IViewProvider<UserProfile>, UserViewProvider>();

            // Add discussion profile views
            services.AddScoped<IViewProviderManager<DiscussUser>, ViewProviderManager<DiscussUser>>();
            services.AddScoped<IViewProvider<DiscussUser>, ProfileViewProvider>();


            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, ReplySubscriber<Reply>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Reply>>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // discuss home
            routes.MapAreaRoute(
                name: "Discuss",
                areaName: "Plato.Discuss",
                template: "discuss",
                defaults: new { controller = "Home", action = "Index" }
            );
            
            // discuss popular
            routes.MapAreaRoute(
                name: "DiscussPopular",
                areaName: "Plato.Discuss",
                template: "discuss/popular",
                defaults: new { controller = "Home", action = "Popular" }
            );

            // discuss topic
            routes.MapAreaRoute(
                name: "DiscussTopic",
                areaName: "Plato.Discuss",
                template: "discuss/topic/{id}/{alias}",
                defaults: new { controller = "Home", action = "Topic" }
            );

            // discuss new topic
            routes.MapAreaRoute(
                name: "DiscussNewTopic",
                areaName: "Plato.Discuss",
                template: "discuss/new/{channel?}",
                defaults: new { controller = "Home", action = "Create" }
            );


        }
    }
}