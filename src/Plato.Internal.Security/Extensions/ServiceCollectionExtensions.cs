using System.IO;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Security.Abstractions;

namespace Plato.Internal.Security.Extensions
{
    public static class ServiceCollectionExtensions
    {
        
        public static IServiceCollection AddPlatoAuthorization(
            this IServiceCollection services)
        {
            
            // Permissions manager
            services.AddScoped<IPermissionsManager<Permission>, PermissionsManager<Permission>>();
        
            // Add core authorization services 
            services.AddAuthorization();

            return services;

        }

        public static IServiceCollection AddPlatoDataProtection(
            this IServiceCollection services)
        {

            var platoOptions = services.BuildServiceProvider().GetService<IOptions<PlatoOptions>>();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Secrets");

            if (platoOptions != null)
            {
                if (!string.IsNullOrEmpty(platoOptions.Value.SecretsPath))
                {
                    path = platoOptions.Value.SecretsPath;
                }
            }

            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(path));

            return services;
        }


    }

}
