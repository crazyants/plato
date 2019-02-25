using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Articles.Handlers;
using Plato.Articles.Assets;
using Plato.Articles.Badges;
using Plato.Articles.Models;
using Plato.Articles.Navigation;
using Plato.Articles.Services;
using Plato.Articles.Subscribers;
using Plato.Articles.Tasks;
using Plato.Articles.ViewProviders;
using Plato.Entities.Repositories;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.Subscribers;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Badges;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Articles
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
            services.AddScoped<INavigationProvider, SearchMenu>();
            services.AddScoped<INavigationProvider, ArticleMenu>();
            services.AddScoped<INavigationProvider, ArticleCommentMenu>();

            // Stores
            services.AddScoped<IEntityRepository<Article>, EntityRepository<Article>>();
            services.AddScoped<IEntityStore<Article>, EntityStore<Article>>();
            services.AddScoped<IEntityManager<Article>, EntityManager<Article>>();

            services.AddScoped<IEntityReplyRepository<ArticleComment>, EntityReplyRepository<ArticleComment>>();
            services.AddScoped<IEntityReplyStore<ArticleComment>, EntityReplyStore<ArticleComment>>();
            services.AddScoped<IEntityReplyManager<ArticleComment>, EntityReplyManager<ArticleComment>>();

            // Register data access
            services.AddScoped<IPostManager<Article>, ArticleManager>();
            services.AddScoped<IPostManager<ArticleComment>, ReplyManager>();

            // Services
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<IReplyService, ReplyService>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // Register client resources
            services.AddScoped<IAssetProvider, AssetProvider>();
            
            // Register view providers
            services.AddScoped<IViewProviderManager<Article>, ViewProviderManager<Article>>();
            services.AddScoped<IViewProvider<Article>, ArticleViewProvider>();
            services.AddScoped<IViewProviderManager<ArticleComment>, ViewProviderManager<ArticleComment>>();
            services.AddScoped<IViewProvider<ArticleComment>, CommentViewProvider>();

            // Add discussion views to base user pages
            services.AddScoped<IViewProviderManager<UserProfile>, ViewProviderManager<UserProfile>>();
            services.AddScoped<IViewProvider<UserProfile>, UserViewProvider>();
            
            // Add discussion profile views
            services.AddScoped<IViewProviderManager<DiscussUser>, ViewProviderManager<DiscussUser>>();
            services.AddScoped<IViewProvider<DiscussUser>, ProfileViewProvider>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, ReplySubscriber<ArticleComment>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<ArticleComment>>();

            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, TopicBadges>();
            services.AddScoped<IBadgesProvider<Badge>, ReplyBadges>();

            // Background tasks
            services.AddScoped<IBackgroundTaskProvider, TopicBadgesAwarder>();
            services.AddScoped<IBackgroundTaskProvider, ReplyBadgesAwarder>();


        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // discuss home
            routes.MapAreaRoute(
                name: "Articles",
                areaName: "Plato.Articles",
                template: "articles/{offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );
            
            // discuss popular
            routes.MapAreaRoute(
                name: "ArticlesPopular",
                areaName: "Plato.Articles",
                template: "articles/popular/{offset:int?}",
                defaults: new { controller = "Home", action = "Popular" }
            );

            // discuss topic
            routes.MapAreaRoute(
                name: "ArticlesArticle",
                areaName: "Plato.Articles",
                template: "articles/a/{id:int}/{alias}/{offset:int?}",
                defaults: new { controller = "Home", action = "Topic" }
            );
                     
            // discuss new article
            routes.MapAreaRoute(
                name: "ArticleNew",
                areaName: "Plato.Articles",
                template: "articles/new/{channel?}",
                defaults: new { controller = "Home", action = "Create" }
            );

            // article jump to comment
            routes.MapAreaRoute(
                name: "ArticlesJumpToComment",
                areaName: "Plato.Articles",
                template: "articles/j/{id:int}/{alias}/{replyId:int?}",
                defaults: new { controller = "Home", action = "Jump" }
            );
        }
    }
}