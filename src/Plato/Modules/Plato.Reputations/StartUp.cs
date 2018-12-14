using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Reputations.Handlers;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Tasks.Abstractions;
using Plato.Reputations.Tasks;

namespace Plato.Reputations
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
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // Points & Rank Aggregator background tasks
            services.AddScoped<IBackgroundTaskProvider, UserRankAggregator>();
            services.AddScoped<IBackgroundTaskProvider, UserReputationAggregator>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}