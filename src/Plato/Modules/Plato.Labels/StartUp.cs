using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Labels.Handlers;
using Plato.Labels.Models;
using Plato.Labels.Repositories;
using Plato.Labels.Services;
using Plato.Labels.Stores;

namespace Plato.Labels
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
            
            //// Feature event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Repositories
            services.AddScoped<ILabelRepository<Label>, LabelRepository<Label>>();
            services.AddScoped<ILabelDataRepository<LabelData>, LabelDataRepository>();
            services.AddScoped<ILabelRoleRepository<LabelRole>, LabelRoleRepository>();
            services.AddScoped<IEntityLabelRepository<EntityLabel>, EntityLabelRepository>();

            // Stores
            services.AddScoped<ILabelStore<Label>, LabelStore<Label>>();
            services.AddScoped<ILabelDataStore<LabelData>, LabelDataStore>();
            services.AddScoped<ILabelRoleStore<LabelRole>, LabelRoleStore>();
            services.AddScoped<IEntityLabelStore<EntityLabel>, EntityLabelStore>();

            // Managers
            services.AddScoped<ILabelManager<Label>, LabelManager<Label>>();
        
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}