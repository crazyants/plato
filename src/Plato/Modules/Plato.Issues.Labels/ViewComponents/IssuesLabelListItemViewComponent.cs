using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Issues.Labels.Models;
using Plato.Labels.ViewModels;

namespace Plato.Issues.Labels.ViewComponents
{
 
    public class IssuesLabelListItemViewComponent : ViewComponent
    {
        
        public IssuesLabelListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(
            LabelListItemViewModel<Label> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}
