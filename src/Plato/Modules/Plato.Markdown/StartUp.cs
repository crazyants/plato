using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewAdapters;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Assets.Abstractions;
using Plato.Markdown.Assets;
using Plato.Markdown.Services;
using Plato.Markdown.Subscribers;
using Plato.Markdown.ViewAdaptors;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Text.Abstractions;

namespace Plato.Markdown
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
            
            // Register markdown abstractions
            services.AddSingleton<IMarkdownParserFactory, MarkdownParserFactory>();

            // Register view adaptors
            services.AddScoped<IViewAdapterProvider, EditorViewAdapterProvider>();
            
            // Register client resources
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, ParseEntityHtmlSubscriber>();

            // Replace the IDefaultHtmlEncoder implementation 
            // This ensures HTML is not HtmlEncoded before it's passed to the markdown parser
            // The markdown parser will safely encode the html produced from the supplied markdown
            services.Replace<IDefaultHtmlEncoder, MarkdownHtmlEncoder>(ServiceLifetime.Singleton);
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "PlatoMarkdownWebApi",
                areaName: "Plato.Markdown",
                template: "api/markdown/{controller}/{action}/{id?}",
                defaults: new { controller = "Parse", action = "Get" }
            );

        }
    }
}