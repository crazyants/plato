using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.FileSystem.AppData;
using Plato.Internal.Models.Shell;
using Plato.Internal.Shell.Abstractions;
using Plato.Internal.Stores.Abstractions.Shell;

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

            // ----------------
            // core module management
            // ----------------

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
        
        public static IServiceCollection AddSetFeaturesDescriptor(
            this IServiceCollection services,
            IEnumerable<ShellFeature> shellFeatures)
        {
            services.AddSingleton<IShellDescriptorStore>(new SetShellDescriptorManager(shellFeatures));

            return services;
        }
        
    }

}
