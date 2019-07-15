using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.Categories.Roles.QueryAdapters;
using Plato.Entities.Models;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Entities.Categories.Roles
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

            // Query adapters to limit access by role
            services.AddScoped<IQueryAdapterProvider<Entity>, EntityQueryAdapter>();
            services.AddScoped<IQueryAdapterProvider<FeatureEntityCount>, FeatureEntityCountQueryAdapter>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}