using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Articles.Categories.Models;
using Plato.Articles.Categories.ViewModels;
using Plato.Articles.Models;
using Plato.Categories.ViewModels;
using Plato.Entities.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Articles.Categories.ViewProviders
{
    public class CategoryViewProvider : BaseViewProvider<ArticleCategory>
    {

        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<ArticleCategory> _categoryStore;
        private readonly ICategoryManager<ArticleCategory> _categoryManager;
        private readonly IFeatureFacade _featureFacade;
        private readonly IActionContextAccessor _actionContextAccessor;

        public CategoryViewProvider(
            IContextFacade contextFacade,
            ICategoryStore<ArticleCategory> categoryStore,
            ICategoryManager<ArticleCategory> categoryManager,
            IFeatureFacade featureFacade,
            IActionContextAccessor actionContextAccessor)
        {
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
            _categoryManager = categoryManager;
            _featureFacade = featureFacade;
            _actionContextAccessor = actionContextAccessor;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(ArticleCategory articleCategory, IViewProviderContext context)
        {

            // Ensure we explicitly set the featureId
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Articles.Categories");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);

            Category category = null;
            if (articleCategory?.Id > 0)
            {
                category = await _categoryStore.GetByIdAsync(articleCategory.Id);
            }

            // channel filter options
            var channelViewOpts = new CategoryIndexOptions
            {
                ChannelId = category?.Id ?? 0
            };
            
            // Get topic index view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Article>)] as EntityIndexViewModel<Article>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Article>).ToString()} has not been registered on the HttpContext!");
            }
            
            var indexViewModel = new CategoryIndexViewModel()
            {
                Options = channelViewOpts,
                EntityIndexOptions = viewModel?.Options,
                Pager = viewModel?.Pager
            };

            return Views(
                View<Category>("Home.Index.Header", model => category).Zone("header").Order(1),
                View<Category>("Home.Index.Tools", model => category).Zone("tools").Order(1),
                View<CategoryIndexViewModel>("Home.Index.Content", model => indexViewModel).Zone("content").Order(1),
                View<CategoryListViewModel<ArticleCategory>>("Article.Category.Index.Sidebar", model =>
                {
                    //model.SelectedChannelId = channel?.Id ?? 0;
                    model.Options = channelViewOpts;
                    model.Channels = categories;
                    return model;
                }).Zone("sidebar").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(ArticleCategory indexViewModel, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(ArticleCategory category, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(ArticleCategory category, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
    }
}
