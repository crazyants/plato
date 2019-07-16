using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Articles.StopForumSpam.Notifications;
using Plato.Articles.StopForumSpam.NotificationTypes;
using Plato.Articles.StopForumSpam.SpamOperators;
using Plato.Articles.StopForumSpam.ViewProviders;
using Plato.Articles.Models;
using Plato.Internal.Features.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.Articles.StopForumSpam.Handlers;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Articles.StopForumSpam.Navigation;

namespace Plato.Articles.StopForumSpam
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

            // Navigation providers
            services.AddScoped<INavigationProvider, ArticleMenu>();
            services.AddScoped<INavigationProvider, ArticleCommentMenu>();

            // Register spam operations provider
            services.AddScoped<ISpamOperationProvider<SpamOperation>, SpamOperations>();

            // Register spam operator manager for topics
            services.AddScoped<ISpamOperatorManager<Article>, SpamOperatorManager<Article>>();
            services.AddScoped<ISpamOperatorManager<Comment>, SpamOperatorManager<Comment>>();

            // Register spam operators
            services.AddScoped<ISpamOperatorProvider<Article>, ArticleOperator>();
            services.AddScoped<ISpamOperatorProvider<Comment>, CommentOperator>();

            // Register view providers
            services.AddScoped<IViewProviderManager<Article>, ViewProviderManager<Article>>();
            services.AddScoped<IViewProvider<Article>, ArticleViewProvider>();
            services.AddScoped<IViewProviderManager<Comment>, ViewProviderManager<Comment>>();
            services.AddScoped<IViewProvider<Comment>, CommentViewProvider>();
            
            // Notification types
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification manager
            services.AddScoped<INotificationManager<Article>, NotificationManager<Article>>();
            services.AddScoped<INotificationManager<Comment>, NotificationManager<Comment>>();

            // Notification providers
            services.AddScoped<INotificationProvider<Article>, ArticleSpamWeb>();
            services.AddScoped<INotificationProvider<Article>, ArticleSpamEmail>();
            services.AddScoped<INotificationProvider<Comment>, CommentSpamWeb>();
            services.AddScoped<INotificationProvider<Comment>, CommentSpamEmail>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Index
            routes.MapAreaRoute(
                name: "ArticlesSpamIndex",
                areaName: "Plato.Articles.StopForumSpam",
                template: "articles/a/spam/details/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            // AddSpammer
            routes.MapAreaRoute(
                name: "ArticlesSpamSubmit",
                areaName: "Plato.Articles.StopForumSpam",
                template: "articles/a/spam/add/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "AddSpammer" }
            );
            
        }

    }

}