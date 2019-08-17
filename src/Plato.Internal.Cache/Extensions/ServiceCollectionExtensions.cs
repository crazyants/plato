using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Cache.Abstractions;


namespace Plato.Internal.Cache.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoCaching(
            this IServiceCollection services)
        {

            // Same as services.AddMemoryCache();
             services.Add(ServiceDescriptor.Singleton<IMemoryCache, MemoryCache>());

            // Not implemented - Can be swapped out with real RedisCache
            // I.e. services.AddDistributedRedisCache(options => {});
            services.Add(ServiceDescriptor.Transient<IDistributedCache, MemoryDistributedCache>());

            // Add cache implementations as singletons
            
            services.AddSingleton<ICacheManager, SimpleCacheManager>();
            //services.AddSingleton<ICacheManager, CacheManager>();
            services.AddSingleton<ICacheDependency, CacheDependency>();

            return services;

        }


    }
}
