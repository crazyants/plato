using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Docs.Labels.Models;
using Plato.Labels.ViewModels;

namespace Plato.Docs.Labels.ViewComponents
{
 
    public class DocsLabelListItemViewComponent : ViewComponent
    {
        
        public DocsLabelListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(
            LabelListItemViewModel<Label> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}
