using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Text.Abstractions;
using Plato.Internal.Text.Alias;

namespace Plato.Internal.Text.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoText(
            this IServiceCollection services)
        {

            services.TryAddSingleton<IAliasCreator, AliasCreator>();

            return services;

        }


    }
}
