using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Plato.Hosting.Web.Expanders;
using System.Reflection;
using Plato.Modules.Simple;

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




                // load view components

                var embeddedFileProviders = new EmbeddedFileProvider(
                      typeof(SimpleViewComponent).GetTypeInfo().Assembly,
                      "Plato.Modules.Simple"
                  );

                options.FileProviders.Add(new CompositeFileProvider(embeddedFileProviders));
                
            });

        

            return services;

        }

    }
    
}
