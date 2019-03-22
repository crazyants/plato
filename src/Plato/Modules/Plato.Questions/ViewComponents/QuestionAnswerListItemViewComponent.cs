using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Questions.Models;
using Plato.Entities.ViewModels;

namespace Plato.Questions.ViewComponents
{
    public class QuestionAnswerListItemViewComponent : ViewComponent
    {
        
        public Task<IViewComponentResult> InvokeAsync(
            EntityReplyListItemViewModel<Question, Answer> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}


