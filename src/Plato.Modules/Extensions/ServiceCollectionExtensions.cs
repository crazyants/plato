using Microsoft.AspNetCore.Mvc.Razor;
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
            this IServiceCollection services,
            IMvcCoreBuilder mvcCoreBuilder)
        {

            services.AddSingleton<IConfigureOptions<ModuleOptions>, ModuleOptionsConfigure>();
            services.AddSingleton<IModuleLocator, ModuleLocator>();
            services.AddSingleton<IModuleLoader, ModuleLoader>();
            services.AddSingleton<IModuleManager, ModuleManager>();
                        
            // register dynamically loaded module assemblies with 
            var moduleManager = services.BuildServiceProvider().GetService<IModuleManager>();
            //foreach (ModuleEntry moduleEntry in moduleManager.AvailableModules)
            //{
            //    foreach (var assembly in moduleEntry.Assmeblies)
            //        mvcCoreBuilder.AddApplicationPart(assembly);
            //}
            //mvcCoreBuilder.AddControllersAsServices();

            services.Configure<RazorViewEngineOptions>(configureOptions: options =>
            {                

                // add view location expanders for each available module
                foreach (ModuleEntry moduleEntry in moduleManager.AvailableModules)
                {
                    options.ViewLocationExpanders.Add(new ModuleViewLocationExpander(moduleEntry.Descriptor.ID));
                }

                // invoke context for model binding within dynamic views                                
                var moduleAssemblies = moduleManager.AllAvailableAssemblies.Select(x => MetadataReference.CreateFromFile(x.Location)).ToList();
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
