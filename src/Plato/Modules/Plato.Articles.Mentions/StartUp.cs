using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Articles.Mentions.Notifications;
using Plato.Articles.Mentions.NotificationTypes;
using Plato.Articles.Mentions.Subscribers;
using Plato.Articles.Models;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Articles.Mentions
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
                                         
            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Article>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Comment>>();

            // Register notification providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Article>, NotificationManager<Article>>();
            services.AddScoped<INotificationManager<Comment>, NotificationManager<Comment>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Article>, NewEntityMentionEmail>();
            services.AddScoped<INotificationProvider<Article>, NewEntityMentionWeb>();
            services.AddScoped<INotificationProvider<Comment>, NewReplyMentionWeb>();
            services.AddScoped<INotificationProvider<Comment>, NewReplyMentionEmail>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

        }

    }

}