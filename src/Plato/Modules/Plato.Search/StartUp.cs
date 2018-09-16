using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Search.ViewProviders;
using Plato.WebApi.Controllers;

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

            // Search Discuss view providers
            services.AddScoped<IViewProviderManager<SearchResult>, ViewProviderManager<SearchResult>>();
            services.AddScoped<IViewProvider<SearchResult>, SearchViewProvider>();
            


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
                name: "PlatoSearchApi",
                areaName: "Plato.Search",
                template: "api/search/{controller}/{action}/{id?}",
                defaults: new { controller = "Users", action = "Get" }
            );

        }

    }
}