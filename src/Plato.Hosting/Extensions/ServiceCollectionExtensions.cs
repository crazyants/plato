
using Microsoft.Extensions.DependencyInjection;

namespace Plato.Hosting.Extensions
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
