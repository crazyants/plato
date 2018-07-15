using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Internal.Assets.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoAssets(
            this IServiceCollection services)
        {

            services.TryAddScoped<IAssetManager, AssetManager>();
            
            return services;

        }


    }
}
