using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Plato.Internal.Features.Extensions
{
    public static class ServiceCollectionExtensions
    {
   
        public static IServiceCollection AddPlatoShellFeatures(
            this IServiceCollection services)
        {
            
            services.TryAddScoped<IFeatureEventManager, FeatureEventManager>();

            return services;

        }





    }
}
