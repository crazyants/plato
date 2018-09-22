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
using Plato.Discuss.Labels.ViewAdaptors;
using Plato.Discuss.Labels.ViewProviders;
using Plato.Internal.Layout.ViewAdaptors;

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
            
            // Data stores
            services.AddScoped<ILabelRepository<Models.Label>, LabelRepository<Models.Label>>();
            services.AddScoped<ILabelStore<Models.Label>, LabelStore<Models.Label>>();
            services.AddScoped<ILabelManager<Models.Label>, LabelManager<Models.Label>>();

            // Discuss view providers
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, TopicViewProvider>();

            // Admin view providers
            services.AddScoped<IViewProviderManager<LabelBase>, ViewProviderManager<LabelBase>>();
            services.AddScoped<IViewProvider<LabelBase>, AdminViewProvider>();
       
            // Register view adaptors
            services.AddScoped<IViewAdaptorProvider, TopicListItemViewAdaptor>();

            // Labels view providers
            services.AddScoped<IViewProviderManager<Label>, ViewProviderManager<Label>>();
            services.AddScoped<IViewProvider<Label>, LabelsViewProvider>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "DiscussLabels",
                areaName: "Plato.Discuss.Labels",
                template: "discuss/label/{id}/{alias}",
                defaults: new { controller = "Home", action = "Index" }
            );

        }
    }
}