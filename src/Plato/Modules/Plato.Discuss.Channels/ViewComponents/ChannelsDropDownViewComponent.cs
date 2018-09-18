using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Discuss.Channels.ViewComponents
{

    public class ChannelsDropDownViewComponent : ViewComponent
    {
        private readonly ICategoryStore<Channel> _channelStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;

        public ChannelsDropDownViewComponent(
            ICategoryStore<Channel> channelStore, 
            IContextFacade contextFacade,
            IFeatureFacade featureFacade)
        {
            _channelStore = channelStore;
            _contextFacade = contextFacade;
            _featureFacade = featureFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            IEnumerable<int> selectedChannels, 
            string htmlName)
        {
            if (selectedChannels == null)
            {
                selectedChannels = new int[0];
            }

            var selectedChannelsList = selectedChannels.ToList();
            return View(new SelectChannelsViewModel
            {
                HtmlName = htmlName,
                SelectedChannelIds = selectedChannelsList,
                SelectedChannels = await BuildSelectionsAsync(selectedChannelsList)
            });

        }

        private async Task<IList<Selection<Channel>>> BuildSelectionsAsync(
            IEnumerable<int> selected)
        {

            var feature = await _featureFacade.GetModuleByIdAsync("Plato.Discuss.Channels");
            var channels = await _channelStore.GetByFeatureIdAsync(feature.Id);
            
            var selections = channels?.Select(c => new Selection<Channel>
                {
                    IsSelected = selected.Any(v => v == c.Id),
                    Value = c
                })
                //.OrderBy(s => s.Value.Name)
                .ToList();

            return selections;
        }
    }
    
}

