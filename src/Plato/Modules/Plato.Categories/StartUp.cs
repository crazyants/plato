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
            services.AddScoped<ICategoryRepository<CategoryBase>, CategoryRepository<CategoryBase>>();
            services.AddScoped<ICategoryDataRepository<CategoryData>, CategoryDataRepository>();
            services.AddScoped<ICategoryRoleRepository<CategoryRole>, CategoryRoleRepository>();
            services.AddScoped<IEntityCategoryRepository<EntityCategory>, EntityCategoryRepository>();

            // Stores
            services.AddScoped<ICategoryStore<CategoryBase>, CategoryStore<CategoryBase>>();
            services.AddScoped<ICategoryDataStore<CategoryData>, CategoryDataStore>();
            services.AddScoped<ICategoryRoleStore<CategoryRole>, CategoryRoleStore>();
            services.AddScoped<IEntityCategoryStore<EntityCategory>, EntityCategoryStore>();

            // Managers
            services.AddScoped<ICategoryManager<CategoryBase>, CategoryManager<CategoryBase>>();
        
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}