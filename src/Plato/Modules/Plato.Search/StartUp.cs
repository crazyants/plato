using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;
using Plato.Search.Assets;
using Plato.Search.Models;
using Plato.Search.Navigation;
using Plato.Search.Stores;
using Plato.Search.ViewProviders;
using Plato.WebApi.Controllers;
using Plato.Search.Handlers;

namespace Plato.Search
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
            
            // Stores
            services.AddScoped<ISearchSettingsStore<SearchSettings>, SearchSettingsStore>();

            // Navigation
            services.AddScoped<INavigationProvider, SearchMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();

            // Search Discuss view providers
            services.AddScoped<IViewProviderManager<SearchResult>, ViewProviderManager<SearchResult>>();
            services.AddScoped<IViewProvider<SearchResult>, SearchViewProvider>();
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Register assets
            services.AddScoped<IAssetProvider, AssetProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "PlatoSearch",
                areaName: "Plato.Search",
                template: "search/{keywords?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "PlatoSearchWebApi",
                areaName: "Plato.Search",
                template: "api/{controller}/{action}/{id?}",
                defaults: new { controller = "Users", action = "Get" }
            );

        }

    }
}