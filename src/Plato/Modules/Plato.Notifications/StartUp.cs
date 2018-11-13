using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Notifications.Models;
using Plato.Notifications.Navigation;
using Plato.Notifications.Services;
using Plato.Notifications.ViewModels;
using Plato.Notifications.ViewProviders;

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

            // Managers
            services.AddScoped<INotificationTypeManager<NotificationType>, NotificationTypeManager<NotificationType>>();

            // Default Reaction Providers
            services.AddScoped<INotificationTypeProvider<NotificationType>, DefaultNotiticationTypes>();
            
            // Register navigation provider
            services.AddScoped<INavigationProvider, SiteMenu>();
            services.AddScoped<INavigationProvider, EditProfileMenu>();

            // Edit profile view provider
            services.AddScoped<IViewProviderManager<EditNotificationsViewModel>, ViewProviderManager<EditNotificationsViewModel>>();
            services.AddScoped<IViewProvider<EditNotificationsViewModel>, EditProfileViewProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}