using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Internal.Reputations.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoReputations(
            this IServiceCollection services)
        {
            
            // Dummy reputation awarder
            services.TryAddScoped<IUserReputationAwarder, UserReputationAwarder>();
            
            return services;

        }


    }
}
