using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.History.Handlers;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Entities.History
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

    
            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();


        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}