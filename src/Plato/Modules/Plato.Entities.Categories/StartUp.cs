using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Categories.Stores;
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
            services.AddScoped<IQueryAdapterProvider<EntityQueryParams>, EntityQueryAdapter>();
            services.AddScoped<IQueryAdapterProvider<CategoryQueryParams>, CategoryQueryAdapter>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}