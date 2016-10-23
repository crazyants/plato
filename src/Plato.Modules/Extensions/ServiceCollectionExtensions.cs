﻿using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.CodeAnalysis;
using Plato.Modules.Abstractions;

namespace Plato.Modules.Extensions
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
                        
         
            return services;
        }


    }
}