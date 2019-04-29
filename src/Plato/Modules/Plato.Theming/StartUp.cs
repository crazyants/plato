using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
//using Plato.Internal.Layout.ViewFeatures;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Theming.Abstractions;
using Plato.Theming.Navigation;
using Plato.Theming.Models;
using Plato.Theming.Services;
using Plato.Theming.ViewFeatures;
using Plato.Theming.ViewProviders;

namespace Plato.Theming
{
    public class Startup : StartupBase
    {
        private readonly IShellSettings _shellSettings;

        public Startup(IShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            
            // Register navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // View providers
            services.AddScoped<IViewProviderManager<ThemeAdmin>, ViewProviderManager<ThemeAdmin>>();
            services.AddScoped<IViewProvider<ThemeAdmin>, AdminViewProvider>();

            // Replace dummy site theme services with tenant specific implementations
            services.Replace<ISiteThemeLoader, SiteThemeLoader>(ServiceLifetime.Scoped);
            services.Replace<ISiteThemeFileManager, SiteThemeFileManager>(ServiceLifetime.Scoped);
         
      
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }
}