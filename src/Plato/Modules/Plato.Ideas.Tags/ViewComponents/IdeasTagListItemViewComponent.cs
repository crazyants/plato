using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Ideas.Tags.Models;
using Plato.Tags.ViewModels;

namespace Plato.Ideas.Tags.ViewComponents
{

    public class IdeasTagListItemViewComponent : ViewComponent
    {

   
        public IdeasTagListItemViewComponent()
        {
        
        }
        public Task<IViewComponentResult> InvokeAsync(
            TagListItemViewModel<Tag> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}
