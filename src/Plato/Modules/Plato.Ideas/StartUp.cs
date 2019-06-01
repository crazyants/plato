using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities;
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
using Plato.Ideas.Handlers;
using Plato.Ideas.Assets;
using Plato.Ideas.Badges;
using Plato.Ideas.Models;
using Plato.Ideas.Navigation;
using Plato.Ideas.Notifications;
using Plato.Ideas.NotificationTypes;
using Plato.Ideas.Services;
using Plato.Ideas.Subscribers;
using Plato.Ideas.Tasks;
using Plato.Ideas.ViewProviders;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.Subscribers;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Reputations.Abstractions;
using Plato.Internal.Stores;
using Plato.Internal.Stores.Abstractions.FederatedQueries;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Ideas
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
            services.AddScoped<INavigationProvider, IdeaMenu>();
            services.AddScoped<INavigationProvider, IdeaCommentMenu>();

            // Stores
            services.AddScoped<IEntityRepository<Idea>, EntityRepository<Idea>>();
            services.AddScoped<IEntityStore<Idea>, EntityStore<Idea>>();
            services.AddScoped<IEntityManager<Idea>, EntityManager<Idea>>();

            services.AddScoped<IEntityReplyRepository<IdeaComment>, EntityReplyRepository<IdeaComment>>();
            services.AddScoped<IEntityReplyStore<IdeaComment>, EntityReplyStore<IdeaComment>>();
            services.AddScoped<IEntityReplyManager<IdeaComment>, EntityReplyManager<IdeaComment>>();

            //  Post managers
            services.AddScoped<IPostManager<Idea>, IdeaManager>();
            services.AddScoped<IPostManager<IdeaComment>, IdeaCommentManager>();

            // Entity services
            services.AddScoped<IEntityService<Idea>, EntityService<Idea>>();
            services.AddScoped<IEntityReplyService<IdeaComment>, EntityReplyService<IdeaComment>>();

            // View incrementer
            services.AddScoped<IEntityViewIncrementer<Idea>, EntityViewIncrementer<Idea>>();

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
            services.AddScoped<IViewProviderManager<Idea>, ViewProviderManager<Idea>>();
            services.AddScoped<IViewProvider<Idea>, IdeaViewProvider>();
            services.AddScoped<IViewProviderManager<IdeaComment>, ViewProviderManager<IdeaComment>>();
            services.AddScoped<IViewProvider<IdeaComment>, IdeaCommentViewProvider>();

            //// Add profile views
            //services.AddScoped<IViewProviderManager<Profile>, ViewProviderManager<Profile>>();
            //services.AddScoped<IViewProvider<Profile>, ProfileViewProvider>();
            
            // Add user views
            services.AddScoped<IViewProviderManager<UserIndex>, ViewProviderManager<UserIndex>>();
            services.AddScoped<IViewProvider<UserIndex>, UserViewProvider>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, IdeaSubscriber<Idea>>();
            services.AddScoped<IBrokerSubscriber, IdeaCommentSubscriber<IdeaComment>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<IdeaComment>>();

            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, ArticleBadges>();
            services.AddScoped<IBadgesProvider<Badge>, CommentBadges>();

            // Background tasks
            services.AddScoped<IBackgroundTaskProvider, ArticleBadgesAwarder>();
            services.AddScoped<IBackgroundTaskProvider, CommentBadgesAwarder>();


            // Notification types
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification manager
            services.AddScoped<INotificationManager<ReportSubmission<Idea>>, NotificationManager<ReportSubmission<Idea>>>();
            services.AddScoped<INotificationManager<ReportSubmission<IdeaComment>>, NotificationManager<ReportSubmission<IdeaComment>>>();

            // Notification providers
            services.AddScoped<INotificationProvider<ReportSubmission<Idea>>, IdeaReportWeb>();
            services.AddScoped<INotificationProvider<ReportSubmission<Idea>>, IdeaReportEmail>();
            services.AddScoped<INotificationProvider<ReportSubmission<IdeaComment>>, IdeaCommentReportWeb>();
            services.AddScoped<INotificationProvider<ReportSubmission<IdeaComment>>, IdeaCommentReportEmail>();

            // Report entity managers
            services.AddScoped<IReportEntityManager<Idea>, ReportIdeaManager>();
            services.AddScoped<IReportEntityManager<IdeaComment>, ReportIdeaCommentManager>();

            // Federated query manager 
            services.AddScoped<IFederatedQueryManager<Idea>, FederatedQueryManager<Idea>>();
            services.AddScoped<IFederatedQueryProvider<Idea>, EntitySearchQueries<Idea>>();
          
            // Query adapters
            services.AddScoped<IQueryAdapterManager<Idea>, QueryAdapterManager<Idea>>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Index
            routes.MapAreaRoute(
                name: "Ideas",
                areaName: "Plato.Ideas",
                template: "ideas/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );
            
            // Popular
            routes.MapAreaRoute(
                name: "IdeasPopular",
                areaName: "Plato.Ideas",
                template: "ideas/popular/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Popular" }
            );

            // Entity
            routes.MapAreaRoute(
                name: "IdeasDisplay",
                areaName: "Plato.Ideas",
                template: "ideas/i/{opts.id:int}/{opts.alias}/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Display" }
            );

            // New Entity
            routes.MapAreaRoute(
                name: "IdeasNew",
                areaName: "Plato.Ideas",
                template: "ideas/new/{channel?}",
                defaults: new { controller = "Home", action = "Create" }
            );
            
            // Edit Entity
            routes.MapAreaRoute(
                name: "IdeasEdit",
                areaName: "Plato.Ideas",
                template: "ideas/edit/{opts.id:int?}/{opts.alias?}",
                defaults: new { controller = "Home", action = "Edit" }
            );

            // Display Reply
            routes.MapAreaRoute(
                name: "IdeasDisplayReply",
                areaName: "Plato.Ideas",
                template: "ideas/g/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Reply" }
            );

            // Report 
            routes.MapAreaRoute(
                name: "IdeasReport",
                areaName: "Plato.Ideas",
                template: "ideas/report/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Report" }
            );
            
            // User Index
            routes.MapAreaRoute(
                name: "IdeasUserIndex",
                areaName: "Plato.Ideas",
                template: "u/{opts.createdByUserId:int}/{opts.alias?}/ideas/{pager.offset:int?}",
                defaults: new { controller = "User", action = "Index" }
            );

        }
    }
}