using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Internal.Features.Extensions
{
    public static class ServiceCollectionExtensions
    {
   
        public static IServiceCollection AddPlatoShellFeatures(
            this IServiceCollection services)
        {
            
            services.TryAddScoped<IFeatureEventManager, FeatureEventManager>();
            services.TryAddScoped<IFeatureFacade, FeatureFacade>();

            return services;

        }





    }
}
