using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Categories.Models;
using Plato.Discuss.Channels.ViewAdaptors;
using Plato.Discuss.Channels.ViewModels;
using Plato.Discuss.Channels.ViewProviders;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewAdaptors;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Discuss.Channels
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

            //// Feature installation event handler
            //services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Discuss view providers

            services.AddScoped<IViewProviderManager<DiscussViewModel>, ViewProviderManager<DiscussViewModel>>();
            services.AddScoped<IViewProvider<DiscussViewModel>, DiscussViewProvider>();

            services.AddScoped<IViewProviderManager<ChannelIndexViewModel>, ViewProviderManager<ChannelIndexViewModel>>();
            services.AddScoped<IViewProvider<ChannelIndexViewModel>, ChannelViewProvider>();

            // Admin view providers
            services.AddScoped<IViewProviderManager<Category>, ViewProviderManager<Category>>();
            services.AddScoped<IViewProvider<Category>, AdminViewProvider>();

            // Register view adaptors
            services.AddScoped<IViewAdaptorProvider, DiscussViewAdaptorProvider>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {


            routes.MapAreaRoute(
                name: "DiscussChannel",
                areaName: "Plato.Discuss.Channels",
                template: "discuss/channel/{id}/{alias}",
                defaults: new { controller = "Home", action = "Index" }
            );

        }
    }
}