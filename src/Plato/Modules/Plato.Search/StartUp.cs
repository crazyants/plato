using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Search.Assets;
using Plato.Search.Commands;
using Plato.Search.Models;
using Plato.Search.Navigation;
using Plato.Search.Stores;
using Plato.Search.ViewProviders;
using Plato.Search.Handlers;
using Plato.Search.Repositories;
using Plato.Search.Services;

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
            services.AddScoped<INavigationProvider, SiteSearchMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();
            services.AddScoped<INavigationProvider, AdminMenu>();

            // View providers
            services.AddScoped<IViewProviderManager<SearchResult>, ViewProviderManager<SearchResult>>();
            services.AddScoped<IViewProvider<SearchResult>, SearchViewProvider>();
            
            // Admin view providers
            services.AddScoped<IViewProviderManager<SearchSettings>, ViewProviderManager<SearchSettings>>();
            services.AddScoped<IViewProvider<SearchSettings>, AdminViewProvider>();

            // Register assets
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Repositories
            services.AddScoped<IFullTextCatalogRepository, FullTextCatalogRepository>();
            services.AddScoped<IFullTextIndexRepository, FullTextIndexRepository>();

            // Stores
            services.AddScoped<IFullTextCatalogStore, FullTextCatalogStore>();
            services.AddScoped<IFullTextIndexStore, FullTextIndexStore>();
            
            // Commands
            services.AddScoped<IFullTextCatalogCommand<SchemaFullTextCatalog>, FullTextCatalogCommand>();
            services.AddScoped<IFullTextIndexCommand<SchemaFullTextIndex>, FullTextIndexCommand>();

            // Services
            services.AddScoped<IFullTextCatalogManager, FullTextCatalogManager>();
        
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            // Search index

            routes.MapAreaRoute(
                name: "PlatoSearchIndex",
                areaName: "Plato.Search",
                template: "search/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );
            
            // Admin settings

            routes.MapAreaRoute(
                name: "PlatoSearchAdmin",
                areaName: "Plato.Search",
                template: "admin/settings/search",
                defaults: new { controller = "Admin", action = "Index" }
            );

            // Web Api

            routes.MapAreaRoute(
                name: "PlatoSearchWebApi",
                areaName: "Plato.Search",
                template: "api/{controller}/{action}/{id?}",
                defaults: new { controller = "Search", action = "Get" }
            );

        }

    }
}