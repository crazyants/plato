using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Docs.Categories.Models;
using Plato.Docs.Categories.ViewModels;
using Plato.Docs.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Docs.Categories.ViewProviders
{
    public class CategoryViewProvider : BaseViewProvider<Category>
    {

        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly ICategoryManager<Category> _categoryManager;
        private readonly IFeatureFacade _featureFacade;
        private readonly IActionContextAccessor _actionContextAccessor;

        public CategoryViewProvider(
            IContextFacade contextFacade,
            ICategoryStore<Category> categoryStore,
            ICategoryManager<Category> categoryManager,
            IFeatureFacade featureFacade,
            IActionContextAccessor actionContextAccessor)
        {
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
            _categoryManager = categoryManager;
            _featureFacade = featureFacade;
            _actionContextAccessor = actionContextAccessor;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(Category category, IViewProviderContext context)
        {

            // Ensure we explicitly set the featureId
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs.Categories");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }
            
            Category existingCategory = null;
            if (category?.Id > 0)
            {
                existingCategory = await _categoryStore.GetByIdAsync(category.Id);
            }

            // Get topic index view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Doc>)] as EntityIndexViewModel<Doc>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Doc>).ToString()} has not been registered on the HttpContext!");
            }
            
            // channel filter options
            var channelViewOpts = new CategoryIndexOptions
            {
                CategoryId = existingCategory?.Id ?? 0,
                FeatureId = feature.Id
            };
            
            var indexViewModel = new CategoryIndexViewModel()
            {
                Options = channelViewOpts,
                EntityIndexOptions = viewModel?.Options,
                Pager = viewModel?.Pager
            };

            return Views(
                View<CategoryBase>("Home.Index.Header", model => existingCategory).Zone("header").Order(1),
                View<CategoryBase>("Home.Index.Tools", model => existingCategory).Zone("tools").Order(1),
                View<CategoryIndexViewModel>("Home.Index.Content", model => indexViewModel).Zone("content").Order(1),
                View<CategoryIndexViewModel>("Doc.Categories.Sidebar", model => indexViewModel).Zone("sidebar").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Category indexViewModel, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Category category, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Category category, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
    }
}
