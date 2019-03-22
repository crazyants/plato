using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Questions.Models;
using Plato.Entities.ViewModels;

namespace Plato.Questions.ViewComponents
{
    public class QuestionListItemViewComponent : ViewComponent
    {
        
        public QuestionListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(
            EntityListItemViewModel<Question> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

