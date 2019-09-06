using System.IO;
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

            // Attempt to get secrets path from appsettings.json file
            // If found register file system storage of private keys
            var opts = services.BuildServiceProvider().GetService<IOptions<PlatoOptions>>();
            if (opts != null)
            {
                if (!string.IsNullOrEmpty(opts.Value.SecretsPath))
                {
                    services.AddDataProtection()
                        .PersistKeysToFileSystem(new DirectoryInfo(opts.Value.SecretsPath));
                }
            }

            return services;

        }

    }

}
