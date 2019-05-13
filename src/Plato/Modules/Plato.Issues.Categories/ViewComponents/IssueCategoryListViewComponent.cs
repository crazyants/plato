using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Issues.Categories.Models;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Features;

namespace Plato.Issues.Categories.ViewComponents
{

    public class IssueCategoryListViewComponent : ViewComponent
    {
        private readonly ICategoryStore<Category> _channelStore;
        private readonly IFeatureFacade _featureFacade;

        public IssueCategoryListViewComponent(
            ICategoryStore<Category> channelStore,
            IFeatureFacade featureFacade)
        {
            _channelStore = channelStore;
            _featureFacade = featureFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(CategoryIndexOptions options)
        {

            if (options == null)
            {
                options = new CategoryIndexOptions();
            }
            
            return View(await GetIndexModel(options));

        }
        
        async Task<CategoryListViewModel<Category>> GetIndexModel(CategoryIndexOptions options)
        {
            var feature = await GetCurrentFeature();
            var categories = await _channelStore.GetByFeatureIdAsync(feature.Id);
            return new CategoryListViewModel<Category>()
            {
                Options = options,
                Categories = categories?.Where(c => c.ParentId == options.CategoryId)
            };
        }

        async Task<IShellFeature> GetCurrentFeature()
        {
            var featureId = "Plato.Issues.Categories";
            var feature = await _featureFacade.GetFeatureByIdAsync(featureId);
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the Id '{featureId}'");
            }
            return feature;
        }

    }
    
}
