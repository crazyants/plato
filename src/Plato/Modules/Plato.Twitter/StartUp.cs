using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Twitter.Configuration;
using Plato.Twitter.Handlers;
using Plato.Twitter.Models;
using Plato.Twitter.Navigation;
using Plato.Twitter.Stores;
using Plato.Twitter.ViewProviders;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Twitter
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

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();
            
            // Configuration
            services.AddTransient<IConfigureOptions<TwitterOptions>, TwitterOptionsConfiguration>();

            // Stores
            services.AddScoped<ITwitterSettingsStore<TwitterSettings>, TwitterSettingsStore>();
         
            // View providers
            services.AddScoped<IViewProviderManager<TwitterSettings>, ViewProviderManager<TwitterSettings>>();
            services.AddScoped<IViewProvider<TwitterSettings>, AdminViewProvider>();
            
            // Permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "PlatoTwitterAdmin",
                areaName: "Plato.Twitter",
                template: "admin/settings/twitter",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}