using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.ViewModels;

namespace Plato.Articles.ViewComponents
{
    public class ArticleCommentListItemViewComponent : ViewComponent
    {
        
        public Task<IViewComponentResult> InvokeAsync(
            TopicReplyListItemViewModel model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

