using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Plato.FileSystem;
using Microsoft.Extensions.FileProviders;
using Plato.Shell.Models;
using Plato.FileSystem.AppData;

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

            // file system

            services.AddSingleton<IFileProvider, PhysicalFileProvider>();
            services.AddSingleton<IPlatoFileSystem, PlatoFileSystem>();
            services.AddSingleton<IPlatoFileSystem, HostedFileSystem>();
            services.AddSingleton<IAppDataFolder, PhysicalAppDataFolder>();

            return services;
        }


    }
}
