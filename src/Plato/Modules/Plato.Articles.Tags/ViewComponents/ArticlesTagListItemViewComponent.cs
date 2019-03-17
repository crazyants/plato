using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Tags.Models;
using Plato.Tags.ViewModels;

namespace Plato.Articles.Tags.ViewComponents
{

    public class ArticlesTagListItemViewComponent : ViewComponent
    {

   
        public ArticlesTagListItemViewComponent()
        {
        
        }
        public Task<IViewComponentResult> InvokeAsync(
            TagListItemViewModel<Tag> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}
