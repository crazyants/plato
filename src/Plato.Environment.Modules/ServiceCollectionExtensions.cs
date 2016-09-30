using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Environment.Modules
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddModuleViewLocationExpanders(
            this IServiceCollection services,
            string virtualPath = "Modules")
        {

            var moduleLocater = services.BuildServiceProvider().GetService<IModuleLocator>();
            var moduleLoder = services.BuildServiceProvider().GetService<IModuleLoader>();

            services.Configure<RazorViewEngineOptions>(configureOptions: options =>
            {
                                
                var moduleDescriptors = moduleLocater.LocateModules(
                  new string[] { virtualPath },
                  "Module",
                  "module.txt",
                  false);

                foreach (ModuleDescriptor module in moduleDescriptors)
                {

                    moduleLoder.LoadModule(module);

                    //foreach (var assembly in assemblies)
                    //{
                    //    var embeddedFileProviders = new EmbeddedFileProvider(
                    //        assembly,
                    //        module.ID
                    //    );

                    //    options.FileProviders.Add(new CompositeFileProvider(embeddedFileProviders));
                    //}

                    options.ViewLocationExpanders.Add(new ModuleViewLocationExpander(module.ID));


                }
                
            });

            return services;
        }


    }
}
