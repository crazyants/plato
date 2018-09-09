using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Locales;

namespace Plato.Internal.Localization.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoLocalization(
            this IServiceCollection services)
        {

            // Available time zones provider
            services.AddSingleton<ITimeZoneProvider, TimeZoneProvider>();

            // Local date time provider
            services.AddScoped<ILocalDateTimeProvider, LocalDateTimeProvider>();
            
            // Locales
            services.AddSingleton<ILocaleLocator, LocaleLocator>();
            services.AddSingleton<ILocaleCompositionStrategy, LocaleCompositionStrategy>();
            services.AddSingleton<ILocaleProvider, LocaleProvider>();
            services.AddSingleton<ILocaleManager, LocaleManager>();
            
            return services;

        }


    }
}
