using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.References.Assets;
using Plato.References.Services;
using Plato.References.Subscribers;

namespace Plato.References
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

            // Parsers
            services.AddScoped<IReferencesTokenizer, ReferencesTokenizer>();
            services.AddScoped<IReferencesParser, ReferencesParser>();

            // Register broker subscribers
            services.AddScoped<IBrokerSubscriber, ParseEntityHtmlSubscriber>();

            // Register assets
            services.AddScoped<IAssetProvider, AssetProvider>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}