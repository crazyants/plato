using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewModels;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Discuss.Channels.ViewComponents
{

    public class ChannelsDropDownViewComponent : ViewComponent
    {
        private readonly ICategoryStore<Channel> _channelStore;
        private readonly IContextFacade _contextFacade;

        public ChannelsDropDownViewComponent(
            ICategoryStore<Channel> channelStore, 
            IContextFacade contextFacade)
        {
            _channelStore = channelStore;
            _contextFacade = contextFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            IEnumerable<int> selectedChannels, 
            string htmlName)
        {
            if (selectedChannels == null)
            {
                selectedChannels = new int[0];
            }
            
            return View(new SelectChannelsViewModel
            {
                HtmlName = htmlName,
                SelectedChannels = await BuildSelectionsAsync(selectedChannels)
            });

        }

        private async Task<IList<Selection<Channel>>> BuildSelectionsAsync(
            IEnumerable<int> selected)
        {

            var feature = await _contextFacade.GetFeatureByModuleIdAsync("Plato.Discuss.Channels");
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

