using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Docs.StopForumSpam.Notifications;
using Plato.Docs.StopForumSpam.NotificationTypes;
using Plato.Docs.StopForumSpam.SpamOperators;
using Plato.Docs.StopForumSpam.ViewProviders;
using Plato.Docs.Models;
using Plato.Internal.Features.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.Docs.StopForumSpam.Handlers;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Docs.StopForumSpam.Navigation;

namespace Plato.Docs.StopForumSpam
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
            services.AddScoped<INavigationProvider, DocMenu>();
            services.AddScoped<INavigationProvider, DocCommentMenu>();

            // Register spam operations provider
            services.AddScoped<ISpamOperationProvider<SpamOperation>, SpamOperations>();

            // Register spam operator manager for topics
            services.AddScoped<ISpamOperatorManager<Doc>, SpamOperatorManager<Doc>>();
            services.AddScoped<ISpamOperatorManager<DocComment>, SpamOperatorManager<DocComment>>();

            // Register spam operators
            services.AddScoped<ISpamOperatorProvider<Doc>, DocOperator>();
            services.AddScoped<ISpamOperatorProvider<DocComment>, CommentOperator>();

            // Register view providers
            services.AddScoped<IViewProviderManager<Doc>, ViewProviderManager<Doc>>();
            services.AddScoped<IViewProvider<Doc>, DocViewProvider>();
            services.AddScoped<IViewProviderManager<DocComment>, ViewProviderManager<DocComment>>();
            services.AddScoped<IViewProvider<DocComment>, CommentViewProvider>();
            
            // Notification types
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification manager
            services.AddScoped<INotificationManager<Doc>, NotificationManager<Doc>>();
            services.AddScoped<INotificationManager<DocComment>, NotificationManager<DocComment>>();

            // Notification providers
            services.AddScoped<INotificationProvider<Doc>, DocSpamWeb>();
            services.AddScoped<INotificationProvider<Doc>, DocSpamEmail>();
            services.AddScoped<INotificationProvider<DocComment>, CommentSpamWeb>();
            services.AddScoped<INotificationProvider<DocComment>, CommentSpamEmail>();

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
                name: "DocsSpamIndex",
                areaName: "Plato.Docs.StopForumSpam",
                template: "docs/d/spam/details/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            // AddSpammer
            routes.MapAreaRoute(
                name: "DocsSpamSubmit",
                areaName: "Plato.Docs.StopForumSpam",
                template: "docs/d/spam/add/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "AddSpammer" }
            );
            
        }

    }

}