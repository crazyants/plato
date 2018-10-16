using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Reputations.Handlers;
using Plato.Reputations.Models;
using Plato.Reputations.Providers;
using Plato.Reputations.Repositories;
using Plato.Reputations.Services;
using Plato.Reputations.Stores;

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
            services.AddScoped<IReputationsAwarder, ReputationsAwarder<Reputation>>();

            // Reputation providers
            services.AddScoped<IReputationsProvider<Reputation>, RepProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Activate all registered reputation awarders
            var awarders = serviceProvider.GetServices<IReputationsAwarder>();
            foreach (var awarder in awarders)
            {
                awarder?.Invoke();
            }

        }
    }
}