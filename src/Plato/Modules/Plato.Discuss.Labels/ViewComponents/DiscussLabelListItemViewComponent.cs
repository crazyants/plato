using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Labels.Models;
using Plato.Labels.ViewModels;

namespace Plato.Discuss.Labels.ViewComponents
{
 
    public class DiscussLabelListItemViewComponent : ViewComponent
    {
        
        public DiscussLabelListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(
            LabelListItemViewModel<Label> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}
