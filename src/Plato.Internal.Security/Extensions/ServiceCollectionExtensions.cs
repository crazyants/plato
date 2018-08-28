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
            services.AddScoped<IPermissionsManager, PermissionsManager>();
            services.AddScoped<IPermissionsManager2<Permission>, PermissionsManager2<Permission>>();

            // Add core authorization services 
            services.AddAuthorization();

            return services;

        }


    }
}
