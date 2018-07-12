using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Categories.Handlers;
using Plato.Categories.Models;
using Plato.Categories.Repositories;
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

            //// Set-up event handler
            //services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            //// Feature event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            //// Repositories
            services.AddScoped<ICategoryRepository<Category>, CategoryRepository>();
            services.AddScoped<ICategoryDataRepository<CategoryData>, CategoryDataRepository>();

            //// Stores
            services.AddScoped<ICategoryStore<Category>, CategoryStore>();
            services.AddScoped<ICategoryDataStore<CategoryData>, CategoryDataStore>();

            //// Managers
            //services.AddScoped<IEntityManager<Entity>, EntityManager>();
            //services.AddScoped<IEntityReplyManager<EntityReply>, EntityReplyManager>();


        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}