using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Features;
using Plato.Internal.Hosting;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Discuss.Handlers;


namespace Plato.Discuss
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

            // register navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "Discussions",
                areaName: "Plato.Discussions",
                template: "discussions/{controller}/{action}/{id?}",
                defaults: new { controller = "Discussions", action = "Index" }
            );

        }
    }
}