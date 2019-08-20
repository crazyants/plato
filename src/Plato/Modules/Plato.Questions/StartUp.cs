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
using Plato.Questions.Handlers;
using Plato.Questions.Assets;
using Plato.Questions.Badges;
using Plato.Questions.Models;
using Plato.Questions.Navigation;
using Plato.Questions.Notifications;
using Plato.Questions.NotificationTypes;
using Plato.Questions.Services;
using Plato.Questions.Subscribers;
using Plato.Questions.Tasks;
using Plato.Questions.ViewProviders;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Entities.Search;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.Subscribers;
using Plato.Internal.Abstractions.Routing;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Reputations.Abstractions;
using Plato.Internal.Stores;
using Plato.Internal.Stores.Abstractions;
using Plato.Internal.Stores.Abstractions.FederatedQueries;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Questions
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
            services.AddScoped<INavigationProvider, QuestionMenu>();
            services.AddScoped<INavigationProvider, QuestionAnswerMenu>();

            // Stores
            services.AddScoped<IEntityRepository<Question>, EntityRepository<Question>>();
            services.AddScoped<IEntityStore<Question>, EntityStore<Question>>();
            services.AddScoped<IEntityManager<Question>, EntityManager<Question>>();

            services.AddScoped<IEntityReplyRepository<Answer>, EntityReplyRepository<Answer>>();
            services.AddScoped<IEntityReplyStore<Answer>, EntityReplyStore<Answer>>();
            services.AddScoped<IEntityReplyManager<Answer>, EntityReplyManager<Answer>>();

            //  Post managers
            services.AddScoped<IPostManager<Question>, QuestionManager>();
            services.AddScoped<IPostManager<Answer>, AnswerManager>();

            // Entity services
            services.AddScoped<IEntityService<Question>, EntityService<Question>>();
            services.AddScoped<IEntityReplyService<Answer>, EntityReplyService<Answer>>();

            // View incrementer
            services.AddScoped<IEntityViewIncrementer<Question>, EntityViewIncrementer<Question>>();

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
            services.AddScoped<IViewProviderManager<Question>, ViewProviderManager<Question>>();
            services.AddScoped<IViewProvider<Question>, QuestionViewProvider>();
            services.AddScoped<IViewProviderManager<Answer>, ViewProviderManager<Answer>>();
            services.AddScoped<IViewProvider<Answer>, AnswerViewProvider>();
          
            // Add user views
            services.AddScoped<IViewProviderManager<UserIndex>, ViewProviderManager<UserIndex>>();
            services.AddScoped<IViewProvider<UserIndex>, UserViewProvider>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, QuestionSubscriber<Question>>();
            services.AddScoped<IBrokerSubscriber, AnswerSubscriber<Answer>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Answer>>();

            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, QuestionBadges>();
            services.AddScoped<IBadgesProvider<Badge>, AnswerBadges>();

            // Background tasks
            services.AddScoped<IBackgroundTaskProvider, QuestionBadgesAwarder>();
            services.AddScoped<IBackgroundTaskProvider, AnswerBadgesAwarder>();


            // Notification types
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification manager
            services.AddScoped<INotificationManager<ReportSubmission<Question>>, NotificationManager<ReportSubmission<Question>>>();
            services.AddScoped<INotificationManager<ReportSubmission<Answer>>, NotificationManager<ReportSubmission<Answer>>>();

            // Notification providers
            services.AddScoped<INotificationProvider<ReportSubmission<Question>>, QuestionReportWeb>();
            services.AddScoped<INotificationProvider<ReportSubmission<Question>>, QuestionReportEmail>();
            services.AddScoped<INotificationProvider<ReportSubmission<Answer>>, AnswerReportWeb>();
            services.AddScoped<INotificationProvider<ReportSubmission<Answer>>, AnswerReportEmail>();

            // Report entity managers
            services.AddScoped<IReportEntityManager<Question>, ReportQuestionManager>();
            services.AddScoped<IReportEntityManager<Answer>, ReportAnswerManager>();
            
            // Federated query manager 
            services.AddScoped<IFederatedQueryManager<Question>, FederatedQueryManager<Question>>();
            services.AddScoped<IFederatedQueryProvider<Question>, EntityQueries<Question>>();
        
            // Query adapters
            services.AddScoped<IQueryAdapterManager<Question>, QueryAdapterManager<Question>>();
            
            // Homepage route providers
            services.AddSingleton<IHomeRouteProvider, HomeRoutes>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Index
            routes.MapAreaRoute(
                name: "Questions",
                areaName: "Plato.Questions",
                template: "questions/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );
            
            // Popular
            routes.MapAreaRoute(
                name: "QuestionsPopular",
                areaName: "Plato.Questions",
                template: "questions/popular/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Popular" }
            );

            // Entity
            routes.MapAreaRoute(
                name: "QuestionsDisplay",
                areaName: "Plato.Questions",
                template: "questions/q/{opts.id:int}/{opts.alias}/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Display" }
            );

            // New Entity
            routes.MapAreaRoute(
                name: "QuestionsNew",
                areaName: "Plato.Questions",
                template: "questions/new/{channel?}",
                defaults: new { controller = "Home", action = "Create" }
            );

            // Edit Entity
            routes.MapAreaRoute(
                name: "QuestionsEdit",
                areaName: "Plato.Questions",
                template: "questions/edit/{opts.id:int?}/{opts.alias?}",
                defaults: new { controller = "Home", action = "Edit" }
            );

            // Display Reply
            routes.MapAreaRoute(
                name: "QuestionsDisplayReply",
                areaName: "Plato.Questions",
                template: "questions/g/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Reply" }
            );

            // Report 
            routes.MapAreaRoute(
                name: "QuestionsReport",
                areaName: "Plato.Questions",
                template: "questions/report/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Report" }
            );
            
            // User Index
            routes.MapAreaRoute(
                name: "QuestionsUserIndex",
                areaName: "Plato.Questions",
                template: "u/{opts.createdByUserId:int}/{opts.alias?}/questions/{pager.offset:int?}",
                defaults: new { controller = "User", action = "Index" }
            );

        }
    }
}