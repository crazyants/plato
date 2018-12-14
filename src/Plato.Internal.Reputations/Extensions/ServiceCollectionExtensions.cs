using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Reputations.Abstractions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Reputations.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoReputations(
            this IServiceCollection services)
        {
            
            // Individual user reputation awarder
            services.TryAddScoped<IUserReputationAwarder, UserReputationAwarder>();

            // User reputation provider manager
            services.TryAddScoped<IReputationsManager<Reputation>, ReputationsManager<Reputation>>();

            // User reputation manager
            services.TryAddScoped<IUserReputationManager<UserReputation>, UserReputationManager>();

            //// Points & Rank Aggregator background tasks
            //services.TryAddScoped<IBackgroundTaskProvider, UserRankAggregator>();
            //services.TryAddScoped<IBackgroundTaskProvider, UserReputationAggregator>();

            return services;

        }


    }
}
