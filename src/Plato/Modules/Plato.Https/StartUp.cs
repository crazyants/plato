using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Plato.Https.Configuration;
using Plato.Https.Models;
using Plato.Https.Stores;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Https
{
    public class Startup : StartupBase
    {

        public override void ConfigureServices(IServiceCollection services)
        {

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Stores
            services.AddScoped<IHttpsSettingsStore<HttpsSettings>, HttpsSettingsStore>();
            
            // Rewrite configuration
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<RewriteOptions>, HttpsRewriteOptionsConfiguration>());

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Options
            var options = new RewriteOptions();

            // Call configure on HttpsRewriteOptionsConfiguration
            serviceProvider.GetService<IConfigureOptions<RewriteOptions>>().Configure(options);

            // Provide options to rewritter
            app.UseRewriter(options);

        }
    }
}