using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Features;

namespace Plato.Discuss.Channels.ViewComponents
{

    public class ChannelListViewComponent : ViewComponent
    {
        private readonly ICategoryStore<Channel> _channelStore;
        private readonly IFeatureFacade _featureFacade;

        public ChannelListViewComponent(
            ICategoryStore<Channel> channelStore,
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
        
        async Task<ChannelListViewModel> GetIndexModel(CategoryIndexOptions options)
        {
            var feature = await GetCurrentFeature();
            var categories = await _channelStore.GetByFeatureIdAsync(feature.Id);
            return new ChannelListViewModel()
            {
                Options = options,
                Channels = categories?.Where(c => c.ParentId == options.ChannelId)
            };
        }

        async Task<IShellFeature> GetCurrentFeature()
        {
            var featureId = "Plato.Discuss.Channels";
            var feature = await _featureFacade.GetFeatureByIdAsync(featureId);
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the Id '{featureId}'");
            }
            return feature;
        }

    }


}
