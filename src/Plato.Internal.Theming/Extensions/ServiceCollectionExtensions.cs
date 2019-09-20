using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Theming.Abstractions;
using Plato.Internal.Theming.Abstractions.Locator;

namespace Plato.Internal.Theming.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoTheming(
            this IServiceCollection services)
        {

            services.AddSingleton<IConfigureOptions<ThemeOptions>, ThemeOptionsConfigure>();
            services.AddSingleton<IThemeLocator, ThemeLocator>();

            services.AddSingleton<IThemeLoader, ThemeLoader>();
            services.AddSingleton<IThemeFileManager, ThemeFileManager>();

            services.AddScoped<IThemeCreator, ThemeCreator>();
            services.AddSingleton<IThemeUpdater, ThemeUpdater>();
         
            // Dummy implementations to mimic IThemeManager, until the theming feature is enabled
            services.AddSingleton<ISiteThemeLoader, DummySiteThemeLoader>();
            services.AddSingleton<ISiteThemeFileManager, DummySiteThemeFileManager>();
            
            return services;

        }

    }

}
