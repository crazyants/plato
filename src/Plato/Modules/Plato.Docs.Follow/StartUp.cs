using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Docs.Follow.Handlers;
using Plato.Docs.Follow.Notifications;
using Plato.Docs.Follow.NotificationTypes;
using Plato.Docs.Follow.QueryAdapters;
using Plato.Docs.Follow.Subscribers;
using Plato.Docs.Follow.ViewProviders;
using Plato.Docs.Models;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Docs.Follow
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

            // View providers
            services.AddScoped<IViewProviderManager<Doc>, ViewProviderManager<Doc>>();
            services.AddScoped<IViewProvider<Doc>, DocViewProvider>();
            services.AddScoped<IViewProviderManager<DocComment>, ViewProviderManager<DocComment>>();
            services.AddScoped<IViewProvider<DocComment>, CommentViewProvider>();
            
            // Follow subscribers
            services.AddScoped<IBrokerSubscriber, FollowSubscriber>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<DocComment>>();

            // Notification type providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Doc>, NotificationManager<Doc>>();
            services.AddScoped<INotificationManager<DocComment>, NotificationManager<DocComment>>();
            
            // Notification Providers
            services.AddScoped<INotificationProvider<Doc>, UpdatedDocEmail>();
            services.AddScoped<INotificationProvider<Doc>, UpdatedDocWeb>();
            services.AddScoped<INotificationProvider<DocComment>, NewCommentEmail>();
            services.AddScoped<INotificationProvider<DocComment>, NewCommentWeb>();
        
            // Query adapters 
            services.AddScoped<IQueryAdapterProvider<Doc>, DocQueryAdapter>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}