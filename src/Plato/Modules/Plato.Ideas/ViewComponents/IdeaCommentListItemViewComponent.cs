using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Ideas.Models;
using Plato.Entities.ViewModels;

namespace Plato.Ideas.ViewComponents
{
    public class IdeaCommentListItemViewComponent : ViewComponent
    {
        
        public Task<IViewComponentResult> InvokeAsync(
            EntityReplyListItemViewModel<Idea, IdeaComment> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}


