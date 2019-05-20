using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Labels.Models;
using Plato.Discuss.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Labels.Stores;
using Plato.Labels.ViewModels;

namespace Plato.Discuss.Labels.ViewComponents
{

    public class TopicLabelsViewComponent : ViewComponent
    {

        private readonly ILabelStore<Label> _labelStore;

        public TopicLabelsViewComponent(
            ILabelStore<Label> labelStore)
        {
            _labelStore = labelStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(Topic entity)
        {

            // Get entity labels
            var labels = await _labelStore.QueryAsync()
                .Take(1, 10)
                .Select<LabelQueryParams>(q =>
                {
                    q.EntityId.Equals(entity.Id);
                })
                .OrderBy("Name", OrderBy.Desc)
                .ToList();
            
            return View(new LabelsViewModel<Label>
            {
                Labels = labels?.Data
            });

        }

    }

}
