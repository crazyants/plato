using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Stores;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Categories.Models;
using Plato.Categories.ViewModels;

namespace Plato.Categories.ViewComponents
{

    public class CategoryDropDownViewComponent : ViewComponent
    {
        private readonly ICategoryStore<Category> _channelStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;

        public CategoryDropDownViewComponent(
            ICategoryStore<Category> channelStore, 
            IContextFacade contextFacade,
            IFeatureFacade featureFacade)
        {
            _channelStore = channelStore;
            _contextFacade = contextFacade;
            _featureFacade = featureFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<int> selectedChannels,  string htmlName)
        {
            if (selectedChannels == null)
            {
                selectedChannels = new int[0];
            }
            
            var selectedChannelsList = selectedChannels.ToList();
            var categories = await BuildSelectionsAsync(selectedChannelsList);
            return View(new CategoryInputViewModel(categories)
            {
                HtmlName = htmlName,
                SelectedCategories = selectedChannelsList
            });

        }

        private async Task<IList<Selection<Category>>> BuildSelectionsAsync(
            IEnumerable<int> selected)
        {

            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Channels");
            var channels = await _channelStore.GetByFeatureIdAsync(feature.Id);
            
            var selections = channels?.Select(c => new Selection<Category>
                {
                    IsSelected = selected.Any(v => v == c.Id),
                    Value = c
                })
                .ToList();

            return selections;
        }
    }
    
}

