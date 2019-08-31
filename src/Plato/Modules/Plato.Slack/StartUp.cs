using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Slack.Handlers;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Slack.Stores;
using Plato.Slack.ViewProviders;
using Plato.Slack.Models;
using Plato.Slack.Navigation;
using Plato.Slack.Configuration;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Security.Abstractions;

namespace Plato.Slack
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
            services.AddTransient<IConfigureOptions<SlackOptions>, SlackOptionsConfiguration>();

            // Stores
            services.AddScoped<ISlackSettingsStore<SlackSettings>, SlackSettingsStore>();

            // View providers
            services.AddScoped<IViewProviderManager<SlackSettings>, ViewProviderManager<SlackSettings>>();
            services.AddScoped<IViewProvider<SlackSettings>, AdminViewProvider>();

            // Permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "PlatoSlackAdmin",
                areaName: "Plato.Slack",
                template: "admin/settings/slack",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }
    }
}