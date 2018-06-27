using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Resources.Abstractions;

namespace Plato.Internal.Resources.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoResources(
            this IServiceCollection services)
        {

            services.TryAddScoped<IResourceManager, ResourceManager>();
            
            return services;

        }


    }
}
