using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewAdaptors;
using Plato.Internal.Resources.Abstractions;
using Plato.Markdown.Resources;
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

            // Register view adaptors
            services.AddScoped<IViewAdaptorProvider, EditorViewAdaptorProvider>();
            
            // Register client resources
            services.AddScoped<IResourceProvider, ResourceProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}