using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Abstractions.Stores;
using Plato.Hosting;
using Plato.Shell.Models;
using Plato.Stores.Settings;


namespace Plato.Admin
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
            
            services.AddScoped<ISiteSettingsStore, SiteSettingsStore>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
       
            routes.MapAreaRoute(
                name: "Admin",
                areaName: "Plato.Admin",
                template: "admin",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }
    }
}