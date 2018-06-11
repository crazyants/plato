using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Modules.Abstractions;
using Plato.Modules.Loader;
using Plato.Internal.Modules.Locator;

namespace Plato.Internal.Modules.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoModules(
            this IServiceCollection services)
        {

            services.AddSingleton<IConfigureOptions<ModuleOptions>, ModuleOptionsConfigure>();
            services.AddSingleton<IModuleLocator, ModuleLocator>();
            services.AddSingleton<IModuleLoader, ModuleLoader>();
            services.AddSingleton<IModuleManager, ModuleManager>();
            services.AddSingleton<ITypedModuleProvider, TypedModuleProvider>();

            return services;
        }


    }
}
