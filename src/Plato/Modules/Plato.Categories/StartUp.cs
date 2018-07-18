using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Categories.Handlers;
using Plato.Categories.Models;
using Plato.Categories.Repositories;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Shell;

namespace Plato.Categories
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
            services.AddScoped<ICategoryRepository<Category>, CategoryRepository<Category>>();
            services.AddScoped<ICategoryDataRepository<CategoryData>, CategoryDataRepository>();
            services.AddScoped<ICategoryRoleRepository<CategoryRole>, CategoryRoleRepository>();

            // Stores
            services.AddScoped<ICategoryStore<Category>, CategoryStore<Category>>();
            services.AddScoped<ICategoryDataStore<CategoryData>, CategoryDataStore>();
            services.AddScoped<ICategoryRoleStore<CategoryRole>, CategoryRoleStore>();

            // Managers
            services.AddScoped<ICategoryManager<Category>, CategoryManager<Category>>();
        
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}