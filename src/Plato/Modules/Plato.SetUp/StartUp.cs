using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Hosting;
using Plato.Hosting.Extensions;
using Plato.Shell.Models;
using Plato.SetUp.Services;

namespace Plato.SetUp
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
            services.AddScoped<ISetUpService, SetUpService>();
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "SetUp",
                areaName: "Plato.SetUp",
                template: "",
                defaults: new { controller = "SetUp", action = "Index" }
            );

        }
    }
}