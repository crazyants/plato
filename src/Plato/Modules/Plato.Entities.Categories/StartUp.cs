using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.Categories.QueryAdapters;
using Plato.Entities.Stores;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Stores;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Categories
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

            // Query adapters
            services.AddScoped<IQueryAdapterManager<EntityQueryParams>, QueryAdapterManager<EntityQueryParams>>();
            services.AddScoped<IQueryAdapterProvider<EntityQueryParams>, EntityQueryParamAdapter>();


        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}