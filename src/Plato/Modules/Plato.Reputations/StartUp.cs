using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Reputations.Abstractions;
using Plato.Internal.Tasks.Abstractions;
using Plato.Reputations.Handlers;
using Plato.Reputations.Repositories;
using Plato.Reputations.Services;
using Plato.Reputations.Stores;
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
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();
            
            // Repositories
            services.AddScoped<IUserReputationsRepository<UserReputation>, UserReputationsRepository>();

            // Stores
            services.AddScoped<IUserReputationsStore<UserReputation>, UserReputationsStore>();
            
            // Services
            services.AddScoped<IReputationsManager<Reputation>, ReputationsManager<Reputation>>();
            services.AddScoped<IUserReputationManager<UserReputation>, UserReputationManager>();


            services.AddScoped<IUserReputationAwarder, UserReputationAwarder>();

            // Background tasks
            services.AddScoped<IBackgroundTaskProvider, UserReputationAggregator>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            //// Activate all awarders within registered reputation providers
            //var awarders = serviceProvider.GetServices<IReputationsAwarder>();
            //foreach (var awarder in awarders)
            //{
            //    awarder?.Invoke();
            //}

            //// Activate user total reputation aggregator
            //var aggregator = serviceProvider.GetService<IUserReputationAggregator>();
            //aggregator?.Invoke();

        }

    }

}