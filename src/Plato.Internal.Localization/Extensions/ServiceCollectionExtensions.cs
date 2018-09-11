using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Locales;

namespace Plato.Internal.Localization.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoLocalization(this IServiceCollection services)
        {

            // Available time zones provider
            services.AddSingleton<ITimeZoneProvider, TimeZoneProvider>();

            // Local date time provider
            services.AddScoped<ILocalDateTimeProvider, LocalDateTimeProvider>();
            
            // Locales
            services.AddScoped<ILocaleLocator, LocaleLocator>();
            services.AddScoped<ILocaleCompositionStrategy, LocaleCompositionStrategy>();
            services.AddScoped<ILocaleProvider, LocaleProvider>();
            services.AddScoped<ILocaleStore, LocaleStore>();
            services.AddScoped<ILocaleWatcher, LocaleWatcher>();

            return services;

        }

        public static void UsePlatoLocalization(this IApplicationBuilder app)
        {
            // Initialize locale directory watcher
            var watcher = app.ApplicationServices.GetRequiredService<ILocaleWatcher>();
            watcher.WatchForChanges();
        }

    }
}
