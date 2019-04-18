using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Search.Abstractions;

namespace Plato.Internal.Search.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoSearch(
            this IServiceCollection services)
        {

            services.TryAddScoped<IFullTextIndexManager, FullTextIndexManager>();
            services.AddSingleton<IFullTextQueryParser, FullTextQueryParser>();

            return services;

        }


    }
}
