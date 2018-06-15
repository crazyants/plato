
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Internal.Hosting.Extensions
{
    public static class ServiceCollectionExtensions
    {
 
        public static IServiceCollection AddPlatoDefaultHost(
            this IServiceCollection services)
        {
            
            services.AddSingleton<DefaultPlatoHost>();
            services.AddSingleton<IPlatoHost>(sp => sp.GetRequiredService<DefaultPlatoHost>());
            
            return services;
        }


    }

}
