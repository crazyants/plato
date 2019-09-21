using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Ideas.Mentions.Notifications;
using Plato.Ideas.Mentions.NotificationTypes;
using Plato.Ideas.Mentions.Subscribers;
using Plato.Ideas.Models;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Ideas.Mentions
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
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Idea>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<IdeaComment>>();

            // Register notification providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Idea>, NotificationManager<Idea>>();
            services.AddScoped<INotificationManager<IdeaComment>, NotificationManager<IdeaComment>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Idea>, NewEntityMentionEmail>();
            services.AddScoped<INotificationProvider<Idea>, NewEntityMentionWeb>();
            services.AddScoped<INotificationProvider<IdeaComment>, NewReplyMentionWeb>();
            services.AddScoped<INotificationProvider<IdeaComment>, NewReplyMentionEmail>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

        }

    }

}