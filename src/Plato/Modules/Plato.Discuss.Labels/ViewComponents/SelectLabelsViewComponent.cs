using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Labels.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Shell.Abstractions;
using Plato.Labels.Stores;

namespace Plato.Discuss.Labels.ViewComponents
{

    public class SelectLabelsViewComponent : ViewComponent
    {
        private readonly ILabelStore<Models.Label> _labelStore;
        private readonly IContextFacade _contextFacade;

        public SelectLabelsViewComponent(
            ILabelStore<Models.Label> labelStore, 
            IContextFacade contextFacade)
        {
            _labelStore = labelStore;
            _contextFacade = contextFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            IEnumerable<int> selectedLabels, 
            string htmlName)
        {
            if (selectedLabels == null)
            {
                selectedLabels = new int[0];
            }
            
            return View(new SelectLabelsViewModel
            {
                HtmlName = htmlName,
                SelectedLabels = await BuildSelectionsAsync(selectedLabels)
            });

        }

        private async Task<IList<Selection<Models.Label>>> BuildSelectionsAsync(
            IEnumerable<int> selected)
        {

            var feature = await _contextFacade.GetFeatureByModuleIdAsync("Plato.Discuss.Labels");
            var labels = await _labelStore.GetByFeatureIdAsync(feature.Id);

            var selections = labels?.Select(l => new Selection<Models.Label>
                {
                    IsSelected = selected.Any(v => v == l.Id),
                    Value = l
                })
                .OrderBy(s => s.Value.Name)
                .ToList();

            return selections;
        }
    }



}

