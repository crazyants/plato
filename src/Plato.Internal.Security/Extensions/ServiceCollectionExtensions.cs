using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Plato.Internal.Security.Abstractions;

namespace Plato.Internal.Security.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoSecurity(
            this IServiceCollection services)
        {

            services.AddScoped<IPermissionsManager, PermissionsManager>();


            services.AddAuthorization();

            return services;

        }


    }
}
