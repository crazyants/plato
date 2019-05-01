using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Issues.Models;
using Plato.Entities.ViewModels;

namespace Plato.Issues.ViewComponents
{
    public class IssueCommentListItemViewComponent : ViewComponent
    {
        
        public Task<IViewComponentResult> InvokeAsync(
            EntityReplyListItemViewModel<Issue, Comment> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}



