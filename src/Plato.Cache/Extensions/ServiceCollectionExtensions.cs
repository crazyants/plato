﻿using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;


namespace Plato.Cache.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddCaching(
            this IServiceCollection services)
        {

            services.Add(ServiceDescriptor.Singleton<IMemoryCache, MemoryCache>());

            services.Add(ServiceDescriptor.Transient<IDistributedCache, MemoryDistributedCache>());

            return services;

        }


    }
}