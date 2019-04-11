using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewAdapters;
using Plato.Internal.Layout.ViewProviders;
using Plato.Questions.Categories.Navigation;
using Plato.Questions.Categories.Subscribers;
using Plato.Internal.Messaging.Abstractions;
using Plato.Categories.Models;
using Plato.Categories.Repositories;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Questions.Categories.Models;
using Plato.Questions.Categories.ViewAdapters;
using Plato.Questions.Categories.ViewProviders;
using Plato.Questions.Models;
using Plato.Questions.Categories.Services;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Questions.Categories
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

            // Discuss view providers
            services.AddScoped<IViewProviderManager<Question>, ViewProviderManager<Question>>();
            services.AddScoped<IViewProvider<Question>, QuestionViewProvider>();
            services.AddScoped<IViewProviderManager<Answer>, ViewProviderManager<Answer>>();
            services.AddScoped<IViewProvider<Answer>, AnswerViewProvider>();

            // Home view provider
            services.AddScoped<IViewProviderManager<Category>, ViewProviderManager<Category>>();
            services.AddScoped<IViewProvider<Category>, CategoryViewProvider>();

            // Admin view providers
            services.AddScoped<IViewProviderManager<CategoryAdmin>, ViewProviderManager<CategoryAdmin>>();
            services.AddScoped<IViewProvider<CategoryAdmin>, AdminViewProvider>();
         
            // Category role view providers
            services.AddScoped<IViewProviderManager<CategoryAdmin>, ViewProviderManager<CategoryAdmin>>();
            services.AddScoped<IViewProvider<CategoryAdmin>, CategoryRolesViewProvider>();

            // Register view adapters
            services.AddScoped<IViewAdapterProvider, QuestionListItemViewAdapter>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Answer>>();
            
            // Channel details updater
            services.AddScoped<ICategoryDetailsUpdater, CategoryDetailsUpdater>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            routes.MapAreaRoute(
                name: "HomeQuestionsCategories",
                areaName: "Plato.Questions.Categories",
                template: "questions/c/{opts.categoryId:int?}/{opts.alias?}/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

        }
    }
}