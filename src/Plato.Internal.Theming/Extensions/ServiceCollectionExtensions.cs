using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Theming.Locator;

namespace Plato.Internal.Theming.Extensions
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

        public static void UseThemeStaticFiles(
            this IApplicationBuilder app, 
            IHostingEnvironment env)
        {

            var options = app.ApplicationServices.GetRequiredService<IOptions<ThemeOptions>>();
            if (options != null)
            {
                var contentPath = Path.Combine(env.ContentRootPath, options.Value.VirtualPathToThemesFolder);
                app.UseStaticFiles(new StaticFileOptions
                {
                    RequestPath = "/" + options.Value.VirtualPathToThemesFolder,
                    FileProvider = new PhysicalFileProvider(contentPath)
                });
            }

        }


    }
}
