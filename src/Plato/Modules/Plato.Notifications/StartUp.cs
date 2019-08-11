using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Migrations.Abstractions;
using Plato.Internal.Notifications.Abstractions;
using Plato.Notifications.Handlers;
using Plato.Notifications.Repositories;
using Plato.Notifications.Services;
using Plato.Notifications.Stores;

namespace Plato.Notifications
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

            // Feature event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();
            
            // Repositories
            services.AddScoped<IUserNotificationsRepository<UserNotification>, UserNotificationsRepository>();

            // Stores
            services.AddScoped<IUserNotificationsStore<UserNotification>, UserNotificationsStore>();
            
            // Replace user notification manager with real implementation
            services.Replace<IUserNotificationsManager<UserNotification>, UserNotificationsManager>(ServiceLifetime.Scoped);

            // Replace user notification type defaults with real implementation
            services.Replace<IUserNotificationTypeDefaults, UserNotificationTypeDefaults>(ServiceLifetime.Scoped);
    
            // Migrations
            services.AddSingleton<IMigrationProvider, Migrations>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "NotificationsWebApi",
                areaName: "Plato.Notifications",
                template: "api/notifications/{controller}/{action}/{id?}",
                defaults: new { controller = "User", action = "Get" }
            );

        }

    }

}