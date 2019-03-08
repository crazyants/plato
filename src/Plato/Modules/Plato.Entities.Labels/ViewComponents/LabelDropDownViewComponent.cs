using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Labels.ViewModels;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Entities.Labels.Models;
using Plato.Entities.Labels.Stores;

namespace Plato.Entities.Labels.ViewComponents
{

    public class LabelDropDownViewComponent : ViewComponent
    {

        private readonly ILabelStore<LabelBase> _labelStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;

        public LabelDropDownViewComponent(
            ILabelStore<LabelBase> labelStore,
            IContextFacade contextFacade,
            IFeatureFacade featureFacade)
        {
            _labelStore = labelStore;
            _contextFacade = contextFacade;
            _featureFacade = featureFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            IEnumerable<int> selectedLabels,
            string htmlName)
        {
            if (selectedLabels == null)
            {
                selectedLabels = new int[0];
            }

            var labels = await BuildSelectionsAsync(selectedLabels);
            return View(new LabelDropDownViewModel
            {
                HtmlName = htmlName,
                LabelsJson = labels.Serialize()
            });

        }

        private async Task<IEnumerable<LabelApiResult>> BuildSelectionsAsync(IEnumerable<int> selected)
        {

            if (selected == null)
            {
                return new List<LabelApiResult>();
            }

            if (((int[])selected).Length == 0)
            {
                return new List<LabelApiResult>();
            }
            
            // Get all labels for selected ids
            var labels = await _labelStore.QueryAsync()
                .Select<LabelQueryParams>(q =>
                {
                    q.Id.IsIn(selected.ToArray());
                })
                .OrderBy("TotalEntities", OrderBy.Desc)
                .ToList();

            // Build results 
            var results = new List<LabelApiResult>();
            if (labels?.Data != null)
            {
                foreach (var label in labels.Data)
                {
                    results.Add(new LabelApiResult()
                    {
                        Id = label.Id,
                        Name = label.Name,
                        Description = label.Description,
                        ForeColor = label.ForeColor,
                        BackColor = label.BackColor
                    });

                }
            }
            return results;

        }

    }

}

