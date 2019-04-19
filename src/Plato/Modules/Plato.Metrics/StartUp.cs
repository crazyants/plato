using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Metrics.Handlers;
using Plato.Metrics.Models;
using Plato.Metrics.Repositories;
using Plato.Metrics.Services;
using Plato.Metrics.Stores;

namespace Plato.Metrics
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

            // Repositories
            services.AddScoped<IMetricsRepository<Metric>, MetricsRepository>();

            // Stores
            services.AddScoped<IMetricsStore<Metric>, MetricsStore>();
      
            // Managers
            services.AddScoped<IMetricsManager<Metric>, MetricsManager>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}