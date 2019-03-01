using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Labels.Models;
using Plato.Labels.Repositories;
using Plato.Labels.Services;
using Plato.Labels.Stores;
using Plato.Discuss.Models;
using Plato.Discuss.Labels.Navigation;
using Plato.Discuss.Labels.Models;
using Plato.Discuss.Labels.Services;
using Plato.Discuss.Labels.ViewAdapters;
using Plato.Discuss.Labels.ViewProviders;
using Plato.Internal.Layout.ViewAdapters;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Labels
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
            services.AddScoped<ILabelRepository<Label>, LabelRepository<Label>>();
            services.AddScoped<ILabelStore<Label>, LabelStore<Label>>();
            services.AddScoped<ILabelManager<Label>, LabelManager<Label>>();

            // Discuss view providers
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, TopicViewProvider>();

            // Admin view providers
            services.AddScoped<IViewProviderManager<LabelBase>, ViewProviderManager<LabelBase>>();
            services.AddScoped<IViewProvider<LabelBase>, AdminViewProvider>();
       
            // Register view adapters
            services.AddScoped<IViewAdapterProvider, TopicListItemViewAdapter>();

            // Labels view providers
            services.AddScoped<IViewProviderManager<Label>, ViewProviderManager<Label>>();
            services.AddScoped<IViewProvider<Label>, LabelViewProvider>();

            // Label service
            services.AddScoped<ILabelService, LabelService>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "DiscussLabelIndex",
                areaName: "Plato.Discuss.Labels",
                template: "discuss/labels/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "DiscussLabelDisplay",
                areaName: "Plato.Discuss.Labels",
                template: "discuss/label/{opts.labelId:int}/{alias?}/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Display" }
            );


        }
    }
}