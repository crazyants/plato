using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Environment.Modules
{
    public static class ServiceCollectionExtensions
    {
                
        public static IServiceCollection AddModuleViewExpanders(
            this IServiceCollection services,          
            string virtualPath)
        {

            var output = services.Configure<ModuleLocatorOptions>(configureOptions: options =>
            {
                var expander = new ModuleLocatorExpander(
                    "Module",
                    new[] { virtualPath },
                    "component.txt"
                    );

                options.ModuleLocatorExpanders.Add(expander);
            });

            return output;
        }


    }
}
