using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;


namespace Plato.Internal.Cache.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoCaching(
            this IServiceCollection services)
        {

            services.Add(ServiceDescriptor.Singleton<IMemoryCache, MemoryCache>());
            services.Add(ServiceDescriptor.Transient<IDistributedCache, MemoryDistributedCache>());

            services.AddSingleton<ICacheDependency, CacheDependency>();

            return services;

        }


    }
}
