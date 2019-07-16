using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Ideas.Follow.Handlers;
using Plato.Ideas.Follow.Notifications;
using Plato.Ideas.Follow.NotificationTypes;
using Plato.Ideas.Follow.QueryAdapters;
using Plato.Ideas.Follow.Subscribers;
using Plato.Ideas.Follow.ViewProviders;
using Plato.Ideas.Models;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Ideas.Follow
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
            services.AddScoped<IViewProviderManager<Idea>, ViewProviderManager<Idea>>();
            services.AddScoped<IViewProvider<Idea>, IdeaViewProvider>();
            services.AddScoped<IViewProviderManager<IdeaComment>, ViewProviderManager<IdeaComment>>();
            services.AddScoped<IViewProvider<IdeaComment>, CommentViewProvider>();
            
            // Follow subscribers
            services.AddScoped<IBrokerSubscriber, FollowSubscriber>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<IdeaComment>>();

            // Notification type providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<IdeaComment>, NotificationManager<IdeaComment>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<IdeaComment>, NewIdeaCommentEmail>();
            services.AddScoped<INotificationProvider<IdeaComment>, NewIdeaCommentWeb>();
        
            // Query adapters 
            services.AddScoped<IQueryAdapterProvider<Idea>, IdeaQueryAdapter>();

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