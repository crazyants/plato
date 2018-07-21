using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Categories.Models;
using Plato.Categories.Repositories;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Discuss.Models;
using Plato.Discuss.Tags.Models;
using Plato.Discuss.Tags.ViewProviders;
using Plato.Discuss.ViewModels;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewAdaptors;
using Plato.Internal.Layout.ViewProviders;
using Plato.Labels.Repositories;
using Plato.Labels.Services;
using Plato.Labels.Stores;

namespace Plato.Discuss.Tags
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
            
            // Data stores
            services.AddScoped<ILabelRepository<Tag>, LabelRepository<Tag>>();
            services.AddScoped<ILabelStore<Tag>, LabelStore<Tag>>();
            services.AddScoped<ILabelManager<Tag>, LabelManager<Tag>>();

            // Discuss view providers
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, DiscussViewProvider>();

            //// Channel view provider
            //services.AddScoped<IViewProviderManager<Channel>, ViewProviderManager<Channel>>();
            //services.AddScoped<IViewProvider<Channel>, ChannelViewProvider>();

            //// Admin view providers
            //services.AddScoped<IViewProviderManager<Category>, ViewProviderManager<Category>>();
            //services.AddScoped<IViewProvider<Category>, AdminViewProvider>();

            //// Register view adaptors
            //services.AddScoped<IViewAdaptorProvider, DiscussViewAdaptorProvider>();
            
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