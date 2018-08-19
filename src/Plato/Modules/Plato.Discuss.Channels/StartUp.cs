using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Categories.Models;
using Plato.Categories.Repositories;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewAdaptors;
using Plato.Discuss.Channels.ViewProviders;
using Plato.Discuss.Models;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewAdaptors;
using Plato.Internal.Layout.ViewProviders;
using Plato.Discuss.Channels.Navigation;
using Plato.Discuss.Channels.Subscribers;
using Plato.Internal.Messaging.Abstractions;

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

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();

            // Data stores
            services.AddScoped<ICategoryRepository<Channel>, CategoryRepository<Channel>>();
            services.AddScoped<ICategoryStore<Channel>, CategoryStore<Channel>>();
            services.AddScoped<ICategoryManager<Channel>, CategoryManager<Channel>>();

            // Discuss view providers
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, TopicViewProvider>();

            // Channel view provider
            services.AddScoped<IViewProviderManager<Channel>, ViewProviderManager<Channel>>();
            services.AddScoped<IViewProvider<Channel>, ChannelViewProvider>();

            // Admin view providers
            services.AddScoped<IViewProviderManager<CategoryBase>, ViewProviderManager<CategoryBase>>();
            services.AddScoped<IViewProvider<CategoryBase>, AdminViewProvider>();

            // Register view adaptors
            services.AddScoped<IViewAdaptorProvider, DiscussViewAdaptorProvider>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Topic>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Reply>>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            routes.MapAreaRoute(
                name: "HomeDiscussChannel",
                areaName: "Plato.Discuss.Channels",
                template: "discuss/channels/{id?}/{alias?}",
                defaults: new { controller = "Home", action = "Index" }
            );

        }
    }
}