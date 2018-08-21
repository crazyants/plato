using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;

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
            
   
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Api routes
            routes.MapAreaRoute(
                name: "PlatoSearchApi",
                areaName: "Plato.WebApi",
                template: "api/search/{controller}/{action}/{id?}",
                defaults: new { controller = "Users", action = "Get" }
            );

        }

    }
}