using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ViewAdapters;
using Plato.Internal.Navigation.Abstractions;
using Plato.Labels.Repositories;
using Plato.Labels.Services;
using Plato.Labels.Stores;
using Plato.Issues.Models;
using Plato.Issues.Labels.Navigation;
using Plato.Issues.Labels.Models;
using Plato.Issues.Labels.ViewAdapters;
using Plato.Issues.Labels.ViewProviders;

namespace Plato.Issues.Labels
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
            services.AddScoped<IViewProviderManager<Issue>, ViewProviderManager<Issue>>();
            services.AddScoped<IViewProvider<Issue>, IssueViewProvider>();

            // Admin view providers
            services.AddScoped<IViewProviderManager<LabelAdmin>, ViewProviderManager<LabelAdmin>>();
            services.AddScoped<IViewProvider<LabelAdmin>, AdminViewProvider>();
       
            // Register view adapters
            services.AddScoped<IViewAdapterProvider, IssueListItemViewAdapter>();

            // Labels view providers
            services.AddScoped<IViewProviderManager<Label>, ViewProviderManager<Label>>();
            services.AddScoped<IViewProvider<Label>, LabelViewProvider>();

            // Label service
            services.AddScoped<ILabelService<Label>, LabelService<Label>>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "IssuesLabelIndex",
                areaName: "Plato.Issues.Labels",
                template: "issues/labels/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "IssuesLabelDisplay",
                areaName: "Plato.Issues.Labels",
                template: "issues/label/{opts.labelId:int}/{opts.alias?}/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Display" }
            );


        }
    }
}