using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Ideas.StopForumSpam.Notifications;
using Plato.Ideas.StopForumSpam.NotificationTypes;
using Plato.Ideas.StopForumSpam.SpamOperators;
using Plato.Ideas.StopForumSpam.ViewProviders;
using Plato.Ideas.Models;
using Plato.Internal.Features.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.Ideas.StopForumSpam.Handlers;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Ideas.StopForumSpam.Navigation;

namespace Plato.Ideas.StopForumSpam
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
            services.AddScoped<INavigationProvider, IdeaMenu>();
            services.AddScoped<INavigationProvider, IdeaCommentMenu>();

            // Register spam operations provider
            services.AddScoped<ISpamOperationProvider<SpamOperation>, SpamOperations>();

            // Register spam operator manager for topics
            services.AddScoped<ISpamOperatorManager<Idea>, SpamOperatorManager<Idea>>();
            services.AddScoped<ISpamOperatorManager<IdeaComment>, SpamOperatorManager<IdeaComment>>();

            // Register spam operators
            services.AddScoped<ISpamOperatorProvider<Idea>, IdeaOperator>();
            services.AddScoped<ISpamOperatorProvider<IdeaComment>, CommentOperator>();

            // Register view providers
            services.AddScoped<IViewProviderManager<Idea>, ViewProviderManager<Idea>>();
            services.AddScoped<IViewProvider<Idea>, IdeaViewProvider>();
            services.AddScoped<IViewProviderManager<IdeaComment>, ViewProviderManager<IdeaComment>>();
            services.AddScoped<IViewProvider<IdeaComment>, CommentViewProvider>();
            
            // Notification types
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification manager
            services.AddScoped<INotificationManager<Idea>, NotificationManager<Idea>>();
            services.AddScoped<INotificationManager<IdeaComment>, NotificationManager<IdeaComment>>();

            // Notification providers
            services.AddScoped<INotificationProvider<Idea>, IdeaSpamWeb>();
            services.AddScoped<INotificationProvider<Idea>, IdeaSpamEmail>();
            services.AddScoped<INotificationProvider<IdeaComment>, CommentSpamWeb>();
            services.AddScoped<INotificationProvider<IdeaComment>, CommentSpamEmail>();

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
                name: "IdeasSpamIndex",
                areaName: "Plato.Ideas.StopForumSpam",
                template: "ideas/i/spam/details/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            // AddSpammer
            routes.MapAreaRoute(
                name: "IdeasSpamSubmit",
                areaName: "Plato.Ideas.StopForumSpam",
                template: "ideas/i/spam/add/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "AddSpammer" }
            );
            
        }

    }

}