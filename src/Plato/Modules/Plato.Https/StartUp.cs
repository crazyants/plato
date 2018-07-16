using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Plato.Https.Configuration;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Https
{
    public class Startup : StartupBase
    {

        public override void ConfigureServices(IServiceCollection services)
        {

            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<RewriteOptions>, HttpsRewriteOptionsConfiguration>());

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            var rewriteOptions = new RewriteOptions();

            // Configure the rewrite options.
            serviceProvider.GetService<IConfigureOptions<RewriteOptions>>().Configure(rewriteOptions);

            app.UseRewriter(rewriteOptions);

        }
    }
}