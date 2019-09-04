using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Questions.Mentions.Notifications;
using Plato.Questions.Mentions.NotificationTypes;
using Plato.Questions.Mentions.Subscribers;
using Plato.Questions.Models;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Questions.Mentions
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
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Question>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Answer>>();

            // Register notification providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Question>, NotificationManager<Question>>();
            services.AddScoped<INotificationManager<Answer>, NotificationManager<Answer>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Question>, NewEntityMentionEmail>();
            services.AddScoped<INotificationProvider<Question>, NewEntityMentionWeb>();
            services.AddScoped<INotificationProvider<Answer>, NewReplyMentionWeb>();
            services.AddScoped<INotificationProvider<Answer>, NewReplyMentionEmail>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

        }

    }

}