using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Questions.Labels.Models;
using Plato.Labels.ViewModels;

namespace Plato.Questions.Labels.ViewComponents
{
 
    public class QuestionsLabelListItemViewComponent : ViewComponent
    {
        
        public QuestionsLabelListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(
            LabelListItemViewModel<Label> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}
