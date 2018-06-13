using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Dicsussions;
using Plato.Internal.Hosting;
using Plato.Internal.Navigation;
using Plato.Internal.Shell.Abstractions.Models;

namespace Plato.Discussions
{
    public class Startup : StartupBase
    {
        private readonly ShellSettings _shellSettings;

        public Startup(ShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public override void ConfigureServices(IServiceCollection services)
        {

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