using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Users.Notifications.Assets;
using Plato.Users.Notifications.Navigation;
using Plato.Users.Notifications.ViewModels;
using Plato.Users.Notifications.ViewProviders;

namespace Plato.Users.Notifications
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

            // Register navigation provider
            services.AddScoped<INavigationProvider, SiteMenu>();
            services.AddScoped<INavigationProvider, EditProfileMenu>();
            
            // Edit profile view provider
            services.AddScoped<IViewProviderManager<EditNotificationsViewModel>, ViewProviderManager<EditNotificationsViewModel>>();
            services.AddScoped<IViewProvider<EditNotificationsViewModel>, EditProfileViewProvider>();
            
            // Register assets
            services.AddScoped<IAssetProvider, AssetProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}