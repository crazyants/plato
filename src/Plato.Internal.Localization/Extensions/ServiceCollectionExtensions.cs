using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Localization.Abstractions;

namespace Plato.Internal.Localization.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoLocalization(
            this IServiceCollection services)
        {

            services.AddSingleton<ITimeZoneProvider, TimeZoneProvider>();
            services.AddScoped<ILocalDateTime, LocalDateTime>();

            return services;

        }


    }
}
