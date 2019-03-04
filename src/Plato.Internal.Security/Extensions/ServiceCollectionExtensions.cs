using Microsoft.AspNetCore.Authorization.Policy;
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
            //services.AddScoped<IPolicyEvaluator, PolicyEvaluator>();

            // Add core authorization services 
            services.AddAuthorization();

            return services;

        }
        
    }

}
