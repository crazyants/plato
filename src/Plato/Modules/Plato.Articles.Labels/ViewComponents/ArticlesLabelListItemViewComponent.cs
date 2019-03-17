using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Labels.Models;
using Plato.Labels.ViewModels;

namespace Plato.Articles.Labels.ViewComponents
{
 
    public class ArticlesLabelListItemViewComponent : ViewComponent
    {
        
        public ArticlesLabelListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(
            LabelListItemViewModel<Label> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}
