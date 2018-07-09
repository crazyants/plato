using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Discuss.Handlers;
using Plato.Discuss.Assets;
using Plato.Discuss.Services;
using Plato.Discuss.ViewProviders;
using Plato.Entities.Models;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Discuss
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

            // Register navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();

            // Register data access
            services.AddScoped<IPostManager<Entity>, TopicManager>();
            services.AddScoped<IPostManager<EntityReply>, ReplyManager>();
            
            // Register client resources
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Register view providers
            services.AddScoped<IViewProviderManager<Entity>, ViewProviderManager<Entity>>();
            services.AddScoped<IViewProvider<Entity>, DiscussViewProvider>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "Discuss",
                areaName: "Plato.Discuss",
                template: "discuss",
                defaults: new { controller = "Home", action = "Index" }
            );

        }
    }
}