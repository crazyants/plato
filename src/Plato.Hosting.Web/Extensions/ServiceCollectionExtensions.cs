using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor;
using Plato.Modules.Simple;
using Plato.FileSystem;
using Plato.Environment.Modules;
using Plato.Hosting.Web.Expanders;
using Plato.Environment.Shell;

namespace Plato.Hosting.Web.Extensions

{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPlato(this IServiceCollection services)
        {

            services.Configure<RazorViewEngineOptions>(configureOptions: options =>
            {

                // dynamically load theme at run-time

                var expander = new ThemeViewLocationExpander("classic");
                options.ViewLocationExpanders.Add(expander);

                // shell context

                services.AddSingleton<IShellContextFactory, ShellContextFactory>();

                // file system

                services.AddTransient<IPlatoFileSystem, PlatodFileSystem>();

                // modules

                services.AddSingleton<IModuleLocator, ModuleLocator>();
                //services.AddSingleton<IModuleLibraryService, ModuleLibraryService>();
                
                // load view components

                var embeddedFileProviders = new EmbeddedFileProvider(
                      typeof(SimpleViewComponent).GetTypeInfo().Assembly,
                      "Plato.Modules.Simple"
                  );

                options.FileProviders.Add(new CompositeFileProvider(embeddedFileProviders));
                
            });
            
            return services;

        }


        public static IApplicationBuilder UsePlato(
         this IApplicationBuilder app,
         IHostingEnvironment env,
         ILoggerFactory loggerFactory)
        {
            app.ConfigureWebHost(env, loggerFactory);
            return app;
        }

    }
    
}
