using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Stores;
using Plato.Articles.Categories.Models;
using Plato.Categories.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Features;

namespace Plato.Articles.Categories.ViewComponents
{

    public class ArticleCategoryListViewComponent : ViewComponent
    {
        private readonly ICategoryStore<ArticleCategory> _channelStore;
        private readonly IFeatureFacade _featureFacade;

        public ArticleCategoryListViewComponent(
            ICategoryStore<ArticleCategory> channelStore,
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
        
        async Task<CategoryListViewModel<ArticleCategory>> GetIndexModel(CategoryIndexOptions options)
        {
            var feature = await GetCurrentFeature();
            var categories = await _channelStore.GetByFeatureIdAsync(feature.Id);
            return new CategoryListViewModel<ArticleCategory>()
            {
                Options = options,
                Channels = categories?.Where(c => c.ParentId == options.ChannelId)
            };
        }

        async Task<IShellFeature> GetCurrentFeature()
        {
            var featureId = "Plato.Articles.Categories";
            var feature = await _featureFacade.GetFeatureByIdAsync(featureId);
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the Id '{featureId}'");
            }
            return feature;
        }

    }


}
