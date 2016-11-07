using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Hosting;
using Plato.Hosting.Extensions;

namespace Plato.WebApi
{
    public class Startup : StartupBase
    {
  

        public Startup()
        {
     
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

        
            routes.MapAreaRoute(
                name: "WebAPI",
                area: "Plato.WebApi",
                template: "api/{controller}/{action}",
                controller: "Users",
                action: "Get"
            );


        }
    }
}