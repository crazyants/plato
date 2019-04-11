using System;
using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Questions.Categories.Models;
using Plato.Questions.Categories.ViewModels;
using Plato.Entities.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Questions.Models;

namespace Plato.Questions.Categories.ViewProviders
{
    public class CategoryViewProvider : BaseViewProvider<Category>
    {
        
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IFeatureFacade _featureFacade;
        
        public CategoryViewProvider(
            ICategoryStore<Category> categoryStore,
            IFeatureFacade featureFacade)
        {
            _categoryStore = categoryStore;
            _featureFacade = featureFacade;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(Category categoryAdmin, IViewProviderContext context)
        {

            // Ensure we explicitly set the featureId
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Questions.Categories");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);

            CategoryBase categoryBase = null;
            if (categoryAdmin?.Id > 0)
            {
                categoryBase = await _categoryStore.GetByIdAsync(categoryAdmin.Id);
            }

            // channel filter options
            var channelViewOpts = new CategoryIndexOptions
            {
                ChannelId = categoryBase?.Id ?? 0
            };
            
            // Get topic index view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Question>)] as EntityIndexViewModel<Question>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Question>).ToString()} has not been registered on the HttpContext!");
            }
            
            var indexViewModel = new CategoryIndexViewModel()
            {
                Options = channelViewOpts,
                EntityIndexOptions = viewModel?.Options,
                Pager = viewModel?.Pager
            };

            return Views(
                View<CategoryBase>("Home.Index.Header", model => categoryBase).Zone("header").Order(1),
                View<CategoryBase>("Home.Index.Tools", model => categoryBase).Zone("tools").Order(1),
                View<CategoryIndexViewModel>("Home.Index.Content", model => indexViewModel).Zone("content").Order(1),
                View<CategoryListViewModel<CategoryAdmin>>("Topic.Channels.Index.Sidebar", model =>
                {
                    //model.SelectedChannelId = channel?.Id ?? 0;
                    model.Options = channelViewOpts;
                    model.Categories = categories;
                    return model;
                }).Zone("sidebar").Order(1)
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
