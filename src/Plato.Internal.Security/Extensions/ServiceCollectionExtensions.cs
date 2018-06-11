using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;

namespace Plato.Internal.Security.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoSecurity(
            this IServiceCollection services)
        {

            services.AddAuthorization();

            return services;

        }


    }
}
