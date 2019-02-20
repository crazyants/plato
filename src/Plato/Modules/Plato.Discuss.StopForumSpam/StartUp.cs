using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Discuss.StopForumSpam.Notifications;
using Plato.Discuss.StopForumSpam.NotificationTypes;
using Plato.Discuss.StopForumSpam.SpamOperators;
using Plato.Discuss.StopForumSpam.ViewProviders;
using Plato.Discuss.Models;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;

namespace Plato.Discuss.StopForumSpam
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

            // Register spam operations provider
            services.AddScoped<ISpamOperationProvider<SpamOperation>, SpamOperations>();

            // Register spam operator manager for topics
            services.AddScoped<ISpamOperatorManager<Topic>, SpamOperatorManager<Topic>>();

            // Register spam operators
            services.AddScoped<ISpamOperatorProvider<Topic>, TopicOperator>();

            // Register view providers
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, TopicViewProvider>();
            
            // Notification types
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification manager
            services.AddScoped<INotificationManager<Topic>, NotificationManager<Topic>>();

            // Notification providers
            services.AddScoped<INotificationProvider<Topic>, NewSpamWeb>();
            services.AddScoped<INotificationProvider<Topic>, NewSpamEmail>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}