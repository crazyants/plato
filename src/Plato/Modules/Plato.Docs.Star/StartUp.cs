using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Messaging.Abstractions;
using Plato.Docs.Star.Subscribers;
using Plato.Docs.Star.ViewProviders;
using Plato.Docs.Models;
using Plato.Docs.Star.Handlers;
using Plato.Docs.Star.QueryAdapters;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Docs.Star
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

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // View providers
            services.AddScoped<IViewProviderManager<Doc>, ViewProviderManager<Doc>>();
            services.AddScoped<IViewProvider<Doc>, DocViewProvider>();

            // Star subscribers
            services.AddScoped<IBrokerSubscriber, StarSubscriber>();
            
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // Query adapters 
            services.AddScoped<IQueryAdapterProvider<Doc>, DocQueryAdapter>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}