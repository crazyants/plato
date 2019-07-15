using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Categories.Models;
using Plato.Categories.Roles.QueryAdapters;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Categories.Roles
{

    public class Startup : StartupBase
    {
  
        public Startup()
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {

            // Query adapters to limit access by role
            services.AddScoped<IQueryAdapterProvider<CategoryBase>, CategoryQueryAdapter>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}