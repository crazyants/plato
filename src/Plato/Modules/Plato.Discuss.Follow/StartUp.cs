using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Discuss.Follow.Notifications;
using Plato.Discuss.Follow.NotificationTypes;
using Plato.Discuss.Follow.Subscribers;
using Plato.Discuss.Follow.ViewProviders;
using Plato.Discuss.Models;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Discuss.Follow
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
                   
            // View providers
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, TopicViewProvider>();

            // Follow subscribers
            services.AddScoped<IBrokerSubscriber, FollowSubscriber>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Reply>>();

            // Notification providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Reply>, NotificationManager<Reply>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Reply>, NewReplyEmail>();
            services.AddScoped<INotificationProvider<Reply>, NewReplyWeb>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
      

        }
    }
}