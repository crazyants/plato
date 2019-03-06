using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Articles.Categories.Models;
using Plato.Articles.Categories.ViewModels;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Articles.Categories.ViewComponents
{

    public class SelectArticleCategoryViewComponent : ViewComponent
    {
        private readonly ICategoryStore<Channel> _channelStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;

        public SelectArticleCategoryViewComponent(
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
            ;
            var model = new SelectChannelsViewModel
            {
                HtmlName = htmlName,
                SelectedChannels = await BuildChannelSelectionsAsync(selectedChannels)
            };

            return View(model);
        }

        private async Task<IList<Selection<Channel>>> BuildChannelSelectionsAsync(
            IEnumerable<int> selectedChannels)
        {
            
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Articles.Categories");
            var channels = await _channelStore.GetByFeatureIdAsync(feature.Id);
            if (channels != null)
            {

                var items = await RecurseChannels(channels);
          
                var selections = items.Select(r => new Selection<Channel>
                    {
                        IsSelected = selectedChannels.Any(v => v == r.Id),
                        Value = r
                    })
                    .OrderBy(r => r.Value)
                    .ToList();

                return selections;

            }

            return null;
        }


        Task<IList<Channel>> RecurseChannels(
            IEnumerable<ICategory> input,
            IList<Channel> output = null,
            int id = 0)
        {

            if (output == null)
            {
                output = new List<Channel>();
            }

            var categories = input.ToList();
            foreach (var category in categories)
            {
                if (category.ParentId == id)
                {
                    var indent = "-".Repeat(category.Depth);
                    if (!string.IsNullOrEmpty(indent))
                    {
                        indent += " ";
                    }
                    output.Add(new Channel
                    {
                        Id = category.Id,
                        Name = indent + category.Name
                       
                    });
                    RecurseChannels(categories, output, category.Id);
                }
            }

            return Task.FromResult(output);

        }

    }

}
