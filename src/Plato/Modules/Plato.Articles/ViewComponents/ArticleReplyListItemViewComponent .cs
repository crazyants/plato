using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Models;
using Plato.Entities.ViewModels;

namespace Plato.Articles.ViewComponents
{
    public class ArticleCommentListItemViewComponent : ViewComponent
    {
        
        public Task<IViewComponentResult> InvokeAsync(
            EntityReplyListItemViewModel<Article, Comment> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

