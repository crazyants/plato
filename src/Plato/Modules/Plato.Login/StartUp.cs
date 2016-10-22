using System;
using Plato.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Plato.Hosting.Extensions;

namespace Plato.Login
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection serviceCollection)
        {

        }

        public override void Configure(IApplicationBuilder builder, IRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaRoute(
                name: "Login",
                area: "Plato.Login",
                template: "admin",
                controller: "Login",
                action: "Index"
            );
        }

    }



}
