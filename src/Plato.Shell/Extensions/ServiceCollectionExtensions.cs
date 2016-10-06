using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Plato.FileSystem;
using Microsoft.Extensions.FileProviders;

namespace Plato.Environment.Shell.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddShell(
            this IServiceCollection services)
        {

            // file system

            services.AddSingleton<IFileProvider, PhysicalFileProvider>();
            services.AddSingleton<IPlatoFileSystem, PlatoFileSystem>();
            services.AddSingleton<IPlatoFileSystem, HostedFileSystem>();

            return services;
        }


    }
}
