using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Docs.Categories.Models;
using Plato.Docs.Categories.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Features;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Docs.Categories.ViewComponents
{

    public class DocCategoryListViewComponent : ViewComponent
    {

        private readonly ICategoryService<Category> _categoryService;
        private readonly IFeatureFacade _featureFacade;

        public DocCategoryListViewComponent(
            IFeatureFacade featureFacade, 
            ICategoryService<Category> categoryService)
        {
            _featureFacade = featureFacade;
            _categoryService = categoryService;
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
            //var feature = await GetCurrentFeature();
            //var categories = await _channelStore.GetByFeatureIdAsync(feature.Id);

            // Get categories
            var categories = await _categoryService
                .GetResultsAsync(options, new PagerOptions()
                {
                    Page = 1,
                    Size = int.MaxValue
                });
            
            return new CategoryListViewModel<Category>()
            {
                Options = options,
                Categories = categories?.Data?.Where(c => c.ParentId == options.CategoryId)
            };
        }

        async Task<IShellFeature> GetCurrentFeature()
        {
            var featureId = "Plato.Docs.Categories";
            var feature = await _featureFacade.GetFeatureByIdAsync(featureId);
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the Id '{featureId}'");
            }
            return feature;
        }

    }


}
