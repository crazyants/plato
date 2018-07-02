using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewAdaptors;
using Plato.Internal.Resources.Abstractions;
using Plato.Markdown.Resources;
using Plato.Markdown.Services;
using Plato.Markdown.ViewAdaptors;

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
            services.AddScoped<IViewAdaptorProvider, EditorViewAdaptorProvider>();
            
            // Register client resources
            services.AddScoped<IResourceProvider, ResourceProvider>();

            // Register message broker
            services.AddSingleton<IMarkdownSubscriber, MarkdownSubscriber>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // set-up parsing event listeners
            var subscriber = app.ApplicationServices.GetRequiredService<IMarkdownSubscriber>();
            subscriber?.Subscribe();
            
            routes.MapAreaRoute(
                name: "PlatoMarkdownParserService",
                areaName: "Plato.Markdown",
                template: "api/{controller}/{action}/{id?}",
                defaults: new { controller = "Parse", action = "Get" }
            );

        }
    }
}