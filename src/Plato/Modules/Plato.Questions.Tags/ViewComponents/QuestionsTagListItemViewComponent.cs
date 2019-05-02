using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Questions.Tags.Models;
using Plato.Tags.ViewModels;

namespace Plato.Questions.Tags.ViewComponents
{

    public class QuestionsTagListItemViewComponent : ViewComponent
    {

   
        public QuestionsTagListItemViewComponent()
        {
        
        }
        public Task<IViewComponentResult> InvokeAsync(
            TagListItemViewModel<Tag> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}
