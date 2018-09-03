using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Security.Abstractions;

namespace Plato.Internal.Security.Extensions
{
    public static class ServiceCollectionExtensions
    {
        
        public static IServiceCollection AddPlatoSecurity(
            this IServiceCollection services)
        {
            
            // Permissions manager
            services.AddScoped<IPermissionsManager<Permission>, PermissionsManager<Permission>>();

            // Add core authorization services 
            services.AddAuthorization();

            return services;

        }


    }
}
