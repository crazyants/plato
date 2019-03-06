using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Stores;
using Plato.Articles.Categories.Models;
using Plato.Articles.Categories.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Features;

namespace Plato.Articles.Categories.ViewComponents
{

    public class ArticleCategoryListViewComponent : ViewComponent
    {
        private readonly ICategoryStore<Channel> _channelStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;

        public ArticleCategoryListViewComponent(
            ICategoryStore<Channel> channelStore,
            IContextFacade contextFacade,
            IFeatureFacade featureFacade)
        {
            _channelStore = channelStore;
            _contextFacade = contextFacade;
            _featureFacade = featureFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            ChannelIndexOptions options)
        {

            if (options == null)
            {
                options = new ChannelIndexOptions();
            }

            var model = await GetIndexModel(options);
            model.SelectedChannelId = options.ChannelId;
            model.ChannelIndexOpts = options;
            return View(model);

        }
        
        async Task<ChannelListViewModel> GetIndexModel(ChannelIndexOptions channelIndexOpts)
        {
            var feature = await GetcurrentFeature();
            var categories = await _channelStore.GetByFeatureIdAsync(feature.Id);
            return new ChannelListViewModel()
            {
                Channels = categories?.Where(c => c.ParentId == channelIndexOpts.ChannelId)
            };
        }

        async Task<IShellFeature> GetcurrentFeature()
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
