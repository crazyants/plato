using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Labels.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Labels.Stores;

namespace Plato.Discuss.Labels.ViewComponents
{

    public class SelectLabelsViewComponent : ViewComponent
    {
        private readonly ILabelStore<Models.Label> _tagStore;
        private readonly IContextFacade _contextFacade;

        public SelectLabelsViewComponent(
            ILabelStore<Models.Label> tagStore, 
            IContextFacade contextFacade)
        {
            _tagStore = tagStore;
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
            
            return View(new SelectTagsViewModel
            {
                HtmlName = htmlName,
                SelectedChannels = await BuildSelectionsAsync(selectedChannels)
            });

        }

        private async Task<IList<Selection<Models.Label>>> BuildSelectionsAsync(
            IEnumerable<int> selected)
        {

            var feature = await _contextFacade.GetFeatureByModuleIdAsync("Plato.Discuss.Channels");
            var channels = await _tagStore.GetByFeatureIdAsync(feature.Id);
            
            var selections = channels.Select(c => new Selection<Models.Label>
                {
                    IsSelected = selected.Any(v => v == c.Id),
                    Value = c
                })
                .OrderBy(s => s.Value.Name)
                .ToList();

            return selections;
        }
    }



}

