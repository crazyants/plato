using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Environment.Modules.Abstractions;
using Microsoft.Extensions.Options;

namespace Plato.Environment.Modules
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddModules(
            this IServiceCollection services)
        {

            services.AddSingleton<IConfigureOptions<ModuleOptions>, ModuleOptionsConfigure>();
            services.AddSingleton<IModuleLocator, ModuleLocator>();
            services.AddSingleton<IModuleLoader, ModuleLoader>();
            services.AddSingleton<IModuleManager, ModuleManager>();
                       
            services.Configure<RazorViewEngineOptions>(configureOptions: options =>
            {
                var moduleManager = services.BuildServiceProvider().GetService<IModuleManager>();
                foreach (ModuleEntry moduleEntry in moduleManager.ModuleEntries)
                {
                    options.ViewLocationExpanders.Add(new ModuleViewLocationExpander(moduleEntry.Descriptor.ID));
                }                
            });

            return services;
        }


    }
}
