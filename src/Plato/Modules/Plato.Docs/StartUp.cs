using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Badges;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Tasks.Abstractions;
using Plato.Docs.Handlers;
using Plato.Docs.Assets;
using Plato.Docs.Badges;
using Plato.Docs.Models;
using Plato.Docs.Navigation;
using Plato.Docs.Services;
using Plato.Docs.Subscribers;
using Plato.Docs.Tasks;
using Plato.Docs.ViewProviders;
using Plato.Entities.Repositories;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.Subscribers;
using Plato.Docs.NotificationTypes;
using Plato.Docs.Notifications;
using Plato.Entities;
using Plato.Entities.Models;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Reputations.Abstractions;
using Plato.Internal.Stores;
using Plato.Internal.Stores.Abstractions;
using Plato.Internal.Stores.Abstractions.FederatedQueries;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Docs
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
            services.AddScoped<INavigationProvider, HomeMenu>();
            services.AddScoped<INavigationProvider, SearchMenu>();
            services.AddScoped<INavigationProvider, PostMenu>();
            services.AddScoped<INavigationProvider, UserEntitiesMenu>();
            services.AddScoped<INavigationProvider, DocMenu>();
            services.AddScoped<INavigationProvider, DocCommentMenu>();

            // Stores
            services.AddScoped<IEntityRepository<Doc>, EntityRepository<Doc>>();
            services.AddScoped<IEntityStore<Doc>, EntityStore<Doc>>();
            services.AddScoped<IEntityManager<Doc>, EntityManager<Doc>>();

            services.AddScoped<IEntityReplyRepository<DocComment>, EntityReplyRepository<DocComment>>();
            services.AddScoped<IEntityReplyStore<DocComment>, EntityReplyStore<DocComment>>();
            services.AddScoped<IEntityReplyManager<DocComment>, EntityReplyManager<DocComment>>();

            // Register data access
            services.AddScoped<IPostManager<Doc>, DocManager>();
            services.AddScoped<IPostManager<DocComment>, ReplyManager>();
            
            // Services
            services.AddScoped<IEntityService<Doc>, EntityService<Doc>>();
            services.AddScoped<IEntityReplyService<DocComment>, EntityReplyService<DocComment>>();

            // View incrementer
            services.AddScoped<IEntityViewIncrementer<Doc>, EntityViewIncrementer<Doc>>();
            
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // Register reputation provider
            services.AddScoped<IReputationsProvider<Reputation>, Reputations>();

            // Register client resources
            services.AddScoped<IAssetProvider, AssetProvider>();
         
            // Register admin view providers
            services.AddScoped<IViewProviderManager<AdminIndex>, ViewProviderManager<AdminIndex>>();
            services.AddScoped<IViewProvider<AdminIndex>, AdminViewProvider>();

            // Register view providers
            services.AddScoped<IViewProviderManager<Doc>, ViewProviderManager<Doc>>();
            services.AddScoped<IViewProvider<Doc>, DocViewProvider>();
            services.AddScoped<IViewProviderManager<DocComment>, ViewProviderManager<DocComment>>();
            services.AddScoped<IViewProvider<DocComment>, DocCommentViewProvider>();

            // Add profile views
            //services.AddScoped<IViewProviderManager<Profile>, ViewProviderManager<Profile>>();
            //services.AddScoped<IViewProvider<Profile>, ProfileViewProvider>();
            
            // Add user views
            services.AddScoped<IViewProviderManager<UserIndex>, ViewProviderManager<UserIndex>>();
            services.AddScoped<IViewProvider<UserIndex>, UserViewProvider>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, DocSubscriber<Doc>>();
            services.AddScoped<IBrokerSubscriber, DocCommentSubscriber<DocComment>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<DocComment>>();

            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, DocBadges>();
            services.AddScoped<IBadgesProvider<Badge>, DocCommentBadges>();

            // Background tasks
            services.AddScoped<IBackgroundTaskProvider, DocBadgesAwarder>();
            services.AddScoped<IBackgroundTaskProvider, DocCommentBadgesAwarder>();

            // Notification types
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification manager
            services.AddScoped<INotificationManager<ReportSubmission<Doc>>, NotificationManager<ReportSubmission<Doc>>>();
            services.AddScoped<INotificationManager<ReportSubmission<DocComment>>, NotificationManager<ReportSubmission<DocComment>>>();

            // Notification providers
            services.AddScoped<INotificationProvider<ReportSubmission<Doc>>, DocReportWeb>();
            services.AddScoped<INotificationProvider<ReportSubmission<Doc>>, DocReportEmail>();
            services.AddScoped<INotificationProvider<ReportSubmission<DocComment>>, CommentReportWeb>();
            services.AddScoped<INotificationProvider<ReportSubmission<DocComment>>, CommentReportEmail>();

            // Report entity managers
            services.AddScoped<IReportEntityManager<Doc>, ReportTopicManager>();
            services.AddScoped<IReportEntityManager<DocComment>, ReportReplyManager>();

            // Federated query manager 
            services.AddScoped<IFederatedQueryManager<Doc>, FederatedQueryManager<Doc>>();
            services.AddScoped<IFederatedQueryProvider<Doc>, EntitySearchQueries<Doc>>();
        
            // Query adapters
            services.AddScoped<IQueryAdapterManager<Doc>, QueryAdapterManager<Doc>>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            // Index
            routes.MapAreaRoute(
                name: "Docs",
                areaName: "Plato.Docs",
                template: "docs/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );
            
            // Popular
            routes.MapAreaRoute(
                name: "DocsPopular",
                areaName: "Plato.Docs",
                template: "docs/popular/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Popular" }
            );

            // Entity
            routes.MapAreaRoute(
                name: "DocsEntity",
                areaName: "Plato.Docs",
                template: "docs/d/{opts.id:int}/{opts.alias}/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Display" }
            );

            // New Entity
            routes.MapAreaRoute(
                name: "DocsNew",
                areaName: "Plato.Docs",
                template: "docs/new/{categoryId:int?}/{parentId:int?}",
                defaults: new { controller = "Home", action = "Create" }
            );

            // Edit Entity
            routes.MapAreaRoute(
                name: "DocsEdit",
                areaName: "Plato.Docs",
                template: "docs/edit/{opts.id:int?}/{opts.alias?}",
                defaults: new { controller = "Home", action = "Edit" }
            );

            // Display Reply
            routes.MapAreaRoute(
                name: "DocsDisplayReply",
                areaName: "Plato.Docs",
                template: "docs/g/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Reply" }
            );

            // Report 
            routes.MapAreaRoute(
                name: "DocsReport",
                areaName: "Plato.Docs",
                template: "docs/report/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Report" }
            );

            // User Index
            routes.MapAreaRoute(
                name: "DocsUser",
                areaName: "Plato.Docs",
                template: "u/{opts.createdByUserId:int}/{opts.alias?}/docs/{pager.offset:int?}",
                defaults: new { controller = "User", action = "Index" }
            );

        }
    }
}