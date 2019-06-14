using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewAdapters;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Messaging.Abstractions;
using Plato.Categories.Models;
using Plato.Categories.Repositories;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Categories.Subscribers;
using Plato.Docs.Categories.Models;
using Plato.Docs.Categories.Navigation;
using Plato.Docs.Categories.Services;
using Plato.Docs.Categories.Subscribers;
using Plato.Docs.Categories.ViewAdapters;
using Plato.Docs.Categories.ViewProviders;
using Plato.Docs.Models;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Docs.Categories
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

            // Repositories
            services.AddScoped<ICategoryDataRepository<CategoryData>, CategoryDataRepository>();
            services.AddScoped<ICategoryRoleRepository<CategoryRole>, CategoryRoleRepository>();
            services.AddScoped<ICategoryRepository<Category>, CategoryRepository<Category>>();

            // Stores
            services.AddScoped<ICategoryDataStore<CategoryData>, CategoryDataStore>();
            services.AddScoped<ICategoryRoleStore<CategoryRole>, CategoryRoleStore>();
            services.AddScoped<ICategoryStore<Category>, CategoryStore<Category>>();
            services.AddScoped<ICategoryManager<Category>, CategoryManager<Category>>();
            services.AddScoped<ICategoryService<Category>, CategoryService<Category>>();
            
            // Discuss view providers
            services.AddScoped<IViewProviderManager<Doc>, ViewProviderManager<Doc>>();
            services.AddScoped<IViewProvider<Doc>, DocViewProvider>();
            services.AddScoped<IViewProviderManager<DocComment>, ViewProviderManager<DocComment>>();
            services.AddScoped<IViewProvider<DocComment>, DocCommentViewProvider>();

            // Home view provider
            services.AddScoped<IViewProviderManager<Category>, ViewProviderManager<Category>>();
            services.AddScoped<IViewProvider<Category>, CategoryViewProvider>();

            // Admin view providers
            services.AddScoped<IViewProviderManager<CategoryAdmin>, ViewProviderManager<CategoryAdmin>>();
            services.AddScoped<IViewProvider<CategoryAdmin>, AdminViewProvider>();
         
            // Register view adapters
            services.AddScoped<IViewAdapterProvider, DocListItemViewAdapter>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Doc>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<DocComment>>();
            services.AddScoped<IBrokerSubscriber, CategorySubscriber<Category>>();

            // Category details updater
            services.AddScoped<ICategoryDetailsUpdater, CategoryDetailsUpdater>();

            // Query adapters
            services.AddScoped<IQueryAdapterManager<Category>, QueryAdapterManager<Category>>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            routes.MapAreaRoute(
                name: "DocsCategoriesIndex",
                areaName: "Plato.Docs.Categories",
                template: "docs/categories/{opts.categoryId:int?}/{opts.alias?}/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

        }
    }
}