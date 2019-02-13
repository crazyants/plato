using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Net.Abstractions;

namespace Plato.Internal.Net.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoNet(
            this IServiceCollection services)
        {

            services.TryAddScoped<IClientIpAddress, ClientIpAddress>();
            
            return services;

        }


    }
}
