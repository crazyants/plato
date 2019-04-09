using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Ideas.Labels.Models;
using Plato.Labels.ViewModels;

namespace Plato.Ideas.Labels.ViewComponents
{
 
    public class IdeasLabelListItemViewComponent : ViewComponent
    {
        
        public IdeasLabelListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(
            LabelListItemViewModel<Label> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}
