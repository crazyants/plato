using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Plato.FileSystem;
using Microsoft.Extensions.FileProviders;
using Plato.FileSystem.Abstractions;
using Plato.Shell.Models;
using Plato.FileSystem.AppData;
using Plato.Shell.Abstractions;

namespace Plato.Shell.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureShell(
            this IServiceCollection services,
            string shellLocation)
        {
            return services.Configure<ShellOptions>(options =>
            {
                options.Location = shellLocation;
            });
        }

        public static IServiceCollection AddFileSystem(
            this IServiceCollection services)
        {
                    
            services.AddSingleton<IAppDataFolder, PhysicalAppDataFolder>();

            return services;
        }


    }
}
