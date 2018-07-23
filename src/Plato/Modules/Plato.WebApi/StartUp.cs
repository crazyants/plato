using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.WebApi.Configuration;
using Plato.WebApi.Filters;
using Plato.WebApi.Middleware;
using Plato.WebApi.Services;
using Plato.WebApi.ViewProviders;

namespace Plato.WebApi
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

            // Rewrite web api configuration
            services.TryAddEnumerable(ServiceDescriptor
                .Transient<IConfigureOptions<WebApiOptions>, WebApiOptionsConfiguration>());

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // View providers
            services.AddScoped<IViewProviderManager<User>, ViewProviderManager<User>>();
            services.AddScoped<IViewProvider<User>, UserViewProvider>();

            // Services
            services.AddScoped<IWebApiAuthenticator, WebApiAuthenticator>();

            // Add anti forgery token for web api requests
            services.Configure<MvcOptions>((options) =>
            {
                options.Filters.Add(typeof(GenerateWebApiAntiforgeryTokenCookie));
            });


        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            // Register client options middleware 
            app.UseMiddleware<WebApiClientOptionsMiddleware>();

        }



    }
}