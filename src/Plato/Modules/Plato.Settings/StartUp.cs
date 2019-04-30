using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Settings.Handlers;
using Plato.Settings.Models;
using Plato.Settings.Navigation;
using Plato.Settings.ViewProviders;

namespace Plato.Settings
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

            // Register setup events
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // View providers
            services.AddScoped<IViewProviderManager<SettingsIndex>, ViewProviderManager<SettingsIndex>>();
            services.AddScoped<IViewProvider<SettingsIndex>, AdminViewProvider>();


        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            routes.MapAreaRoute(
                name: "PlatoSettingsAdmin",
                areaName: "Plato.Settings",
                template: "admin/settings/general",
                defaults: new { controller = "Admin", action = "Index" }
            );
            
        }

    }

}