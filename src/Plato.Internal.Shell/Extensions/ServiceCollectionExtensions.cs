using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.FileSystem;
using Microsoft.Extensions.FileProviders;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Shell.Models;
using Plato.Internal.FileSystem.AppData;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Internal.Shell.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureShell(
            this IServiceCollection services,
            string shellLocation,
            string schemaLocation)
        {
            return services.Configure<ShellOptions>(options =>
            {
                options.Location = shellLocation;
                options.SchemaLocation = schemaLocation;
            });
        }

        public static IServiceCollection AddPlatoShell(
            this IServiceCollection services)
        {
                    
            services.AddSingleton<IAppDataFolder, PhysicalAppDataFolder>();
            
            // shell / tenant context

            services.AddSingleton<IShellSettingsManager, ShellSettingsManager>();
            services.AddSingleton<IShellContextFactory, ShellContextFactory>();
            {
                services.AddSingleton<ICompositionStrategy, CompositionStrategy>();
                services.AddSingleton<IShellContainerFactory, ShellContainerFactory>();
            }

            services.AddSingleton<IRunningShellTable, RunningShellTable>();


            return services;
        }


    }
}
