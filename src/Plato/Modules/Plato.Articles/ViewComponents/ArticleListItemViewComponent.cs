using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Models;
using Plato.Entities.ViewModels;

namespace Plato.Articles.ViewComponents
{
    public class ArticleListItemViewComponent : ViewComponent
    {
        
        public ArticleListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(
            EntityListItemViewModel<Article> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

