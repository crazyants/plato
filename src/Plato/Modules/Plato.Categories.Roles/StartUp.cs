using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Categories.Models;
using Plato.Categories.Roles.QueryAdapters;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Stores.Abstractions.QueryAdapters;
using Plato.Categories.Roles.Services;
using Plato.Internal.Features.Abstractions;
using Plato.Categories.Roles.Handlers;

namespace Plato.Categories.Roles
{

    public class Startup : StartupBase
    {
  
        public Startup()
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Query adapters 
            services.AddScoped<IQueryAdapterProvider<CategoryBase>, CategoryQueryAdapter>();

            // Services
            services.AddScoped<IDefaultCategoryRolesManager, DefaultCategoryRolesManager>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}