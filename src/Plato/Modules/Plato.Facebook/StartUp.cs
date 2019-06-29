using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Facebook.Handlers;
using Plato.Facebook.Models;
using Plato.Facebook.Navigation;
using Plato.Facebook.Stores;
using Plato.Facebook.ViewProviders;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Facebook
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
       
            // Stores
            services.AddScoped<IFacebookSettingsStore<FacebookSettings>, FacebookSettingsStore>();
         
            // View providers
            services.AddScoped<IViewProviderManager<FacebookSettings>, ViewProviderManager<FacebookSettings>>();
            services.AddScoped<IViewProvider<FacebookSettings>, AdminViewProvider>();
            
            // Permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "PlatoFacebookAdmin",
                areaName: "Plato.Facebook",
                template: "admin/settings/facebook",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}