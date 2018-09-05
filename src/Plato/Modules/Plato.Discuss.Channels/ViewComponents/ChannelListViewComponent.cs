using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Shell;

namespace Plato.Discuss.Channels.ViewComponents
{

    public class ChannelListViewComponent : ViewComponent
    {
        private readonly ICategoryStore<Channel> _channelStore;
        private readonly IContextFacade _contextFacade;

        public ChannelListViewComponent(
            ICategoryStore<Channel> channelStore,
            IContextFacade contextFacade)
        {
            _channelStore = channelStore;
            _contextFacade = contextFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            ViewOptions viewOpts)
        {

            if (viewOpts == null)
            {
                viewOpts = new ViewOptions();
            }

            var model = await GetIndexModel(viewOpts);
            model.SelectedChannelId = viewOpts.ChannelId;
            model.ViewOpts = viewOpts;
            return View(model);

        }
        
        async Task<ChannelListViewModel> GetIndexModel(ViewOptions viewOpts)
        {
            var feature = await GetcurrentFeature();
            var categories = await _channelStore.GetByFeatureIdAsync(feature.Id);
            return new ChannelListViewModel()
            {
                Channels = categories?.Where(c => c.ParentId == viewOpts.ChannelId)
            };
        }

        async Task<ShellModule> GetcurrentFeature()
        {
            var featureId = "Plato.Discuss.Channels";
            var feature = await _contextFacade.GetFeatureByModuleIdAsync(featureId);
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the Id '{featureId}'");
            }
            return feature;
        }

    }


}
