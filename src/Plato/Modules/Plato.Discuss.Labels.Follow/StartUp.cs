using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Discuss.Labels.Follow.Notifications;
using Plato.Discuss.Labels.Follow.NotificationTypes;
using Plato.Discuss.Labels.Follow.Subscribers;
using Plato.Discuss.Labels.Follow.ViewProviders;
using Plato.Discuss.Labels.Models;
using Plato.Discuss.Models;
using Plato.Follows.Services;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;
using YamlDotNet.Core.Tokens;


namespace Plato.Discuss.Labels.Follow
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

            // Notification type providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Topic>, NotificationManager<Topic>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Topic>, NewTopicEmail>();
            services.AddScoped<INotificationProvider<Topic>, NewTopicWeb>();

            // View providers
            services.AddScoped<IViewProviderManager<Label>, ViewProviderManager<Label>>();
            services.AddScoped<IViewProvider<Label>, LabelViewProvider>();

            // Subscribers
            services.AddScoped<IBrokerSubscriber, FollowSubscriber>();
            services.AddScoped<IBrokerSubscriber, EntityLabelSubscriber<Topic>>();

            // Follow types
            services.AddScoped<IFollowTypeProvider, FollowTypes>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}