using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Issues.StopForumSpam.Notifications;
using Plato.Issues.StopForumSpam.NotificationTypes;
using Plato.Issues.StopForumSpam.SpamOperators;
using Plato.Issues.StopForumSpam.ViewProviders;
using Plato.Issues.Models;
using Plato.Internal.Features.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.Issues.StopForumSpam.Handlers;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Issues.StopForumSpam.Navigation;

namespace Plato.Issues.StopForumSpam
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
            services.AddScoped<INavigationProvider, IssueMenu>();
            services.AddScoped<INavigationProvider, IssueCommentMenu>();

            // Register spam operations provider
            services.AddScoped<ISpamOperationProvider<SpamOperation>, SpamOperations>();

            // Register spam operator manager for topics
            services.AddScoped<ISpamOperatorManager<Issue>, SpamOperatorManager<Issue>>();
            services.AddScoped<ISpamOperatorManager<Comment>, SpamOperatorManager<Comment>>();

            // Register spam operators
            services.AddScoped<ISpamOperatorProvider<Issue>, IssueOperator>();
            services.AddScoped<ISpamOperatorProvider<Comment>, CommentOperator>();

            // Register view providers
            services.AddScoped<IViewProviderManager<Issue>, ViewProviderManager<Issue>>();
            services.AddScoped<IViewProvider<Issue>, IdeaViewProvider>();
            services.AddScoped<IViewProviderManager<Comment>, ViewProviderManager<Comment>>();
            services.AddScoped<IViewProvider<Comment>, CommentViewProvider>();
            
            // Notification types
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification manager
            services.AddScoped<INotificationManager<Issue>, NotificationManager<Issue>>();
            services.AddScoped<INotificationManager<Comment>, NotificationManager<Comment>>();

            // Notification providers
            services.AddScoped<INotificationProvider<Issue>, IssueSpamWeb>();
            services.AddScoped<INotificationProvider<Issue>, IssueSpamEmail>();
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
                name: "IssuesSpamIndex",
                areaName: "Plato.Issues.StopForumSpam",
                template: "issues/i/spam/details/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            // AddSpammer
            routes.MapAreaRoute(
                name: "IssuesSpamSubmit",
                areaName: "Plato.Issues.StopForumSpam",
                template: "issues/i/spam/add/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "AddSpammer" }
            );
            
        }

    }

}