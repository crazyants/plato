using Microsoft.AspNetCore.Mvc.Razor;
using Plato.Environment.Modules.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Plato.Environment.Modules
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddModules(
            this IServiceCollection services,
            IMvcBuilder mvcBuilder)
        {

            services.AddSingleton<IConfigureOptions<ModuleOptions>, ModuleOptionsConfigure>();
            services.AddSingleton<IModuleLocator, ModuleLocator>();
            services.AddSingleton<IModuleLoader, ModuleLoader>();
            services.AddSingleton<IModuleManager, ModuleManager>();

            //services.AddSingleton<IAssemblyProvider, CompositeModuleProvider>();
            
            var moduleManager = services.BuildServiceProvider().GetService<IModuleManager>();
            foreach (ModuleEntry moduleEntry in moduleManager.ModuleEntries)
            {
                foreach (var assembly in moduleEntry.Assmeblies)                
                    mvcBuilder.AddApplicationPart(assembly);
            }

            mvcBuilder.AddControllersAsServices();
          
            services.Configure<RazorViewEngineOptions>(configureOptions: options =>
            {                
                foreach (ModuleEntry moduleEntry in moduleManager.ModuleEntries)
                {
                    options.ViewLocationExpanders.Add(new ModuleViewLocationExpander(moduleEntry.Descriptor.ID));
                }

                // invoke context for model binding within dynamic views                                
                var moduleAssemblies = moduleManager.AvailableAssemblies.Select(x => MetadataReference.CreateFromFile(x.Location)).ToList();
                var previous = options.CompilationCallback;
                options.CompilationCallback = (context) =>
                {
                    previous?.Invoke(context);
                    context.Compilation = context.Compilation.AddReferences(moduleAssemblies);
                };


            });

            


            return services;
        }


    }
}
