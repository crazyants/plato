using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Theming.Locator;

namespace Plato.Theming.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoTheming(
            this IServiceCollection services)
        {

            services.AddSingleton<IConfigureOptions<ThemeOptions>, ThemeOptionsConfigure>();
            services.AddSingleton<IThemeLocator, ThemeLocator>();
            services.AddSingleton<IThemeManager, ThemeManager>();

            return services;
        }


    }
}
