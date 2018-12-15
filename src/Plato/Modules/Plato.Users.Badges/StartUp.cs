using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Tasks.Abstractions;
using Plato.Notifications.Models;
using Plato.Notifications.Services;
using Plato.Users.Badges.BadgeProviders;
using Plato.Users.Badges.Navigation;
using Plato.Users.Badges.Notifications;
using Plato.Badges.NotificationTypes;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;
using Plato.Users.Badges.Tasks;
using Plato.Users.Badges.ViewProviders;

namespace Plato.Users.Badges
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

            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, VisitBadges>();
            services.AddScoped<IBadgesProvider<Badge>, ProfileBadges>();

            // Background tasks
            services.AddSingleton<IBackgroundTaskProvider, AutobiographerBadgeAwarder>();
            services.AddSingleton<IBackgroundTaskProvider, ConfirmedMemberBadgeAwarder>();
            services.AddSingleton<IBackgroundTaskProvider, VisitBadgesAwarder>();

            // User profile view proviers
            services.AddScoped<IViewProviderManager<UserProfile>, ViewProviderManager<UserProfile>>();
            services.AddScoped<IViewProvider<UserProfile>, UserViewProvider>();

            // Badge view proviers
            services.AddScoped<IViewProviderManager<Badge>, ViewProviderManager<Badge>>();
            services.AddScoped<IViewProvider<Badge>, BadgeViewProvider>();

            // User badges view providers
            services.AddScoped<IViewProviderManager<UserBadge>, ViewProviderManager<UserBadge>>();
            services.AddScoped<IViewProvider<UserBadge>, UserBadgeViewProvider>();

            // Register navigation providers
            services.AddScoped<INavigationProvider, ProfileMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();

            // User notifications Manager
            services.AddScoped<IUserNotificationsManager<UserNotification>, UserNotificationsManager>();
            
            // Notification type providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification manager
            services.AddScoped<INotificationManager<Badge>, NotificationManager<Badge>>();
            
            // Notification Providers
            services.AddScoped<INotificationProvider<Badge>, NewBadgeEmail>();
            services.AddScoped<INotificationProvider<Badge>, NewBadgeWeb>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "BadgesIndex",
                areaName: "Plato.Users.Badges",
                template: "badges",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "DisplayUserBadges",
                areaName: "Plato.Users.Badges",
                template: "users/{id}/{alias?}/badges",
                defaults: new { controller = "Profile", action = "Index" }
            );

        }
    }
}