using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Hosting;
using Plato.Internal.Navigation;

namespace Plato.Modules
{
    public class Startup : StartupBase
    {
    
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
                name: "AdminManageModules",
                areaName: "Plato.Modules",
                template: "admin/modules",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }
    }
}