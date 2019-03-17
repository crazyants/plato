using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Tags.Models;
using Plato.Tags.ViewModels;

namespace Plato.Discuss.Tags.ViewComponents
{

    public class DiscussTagListItemViewComponent : ViewComponent
    {

   
        public DiscussTagListItemViewComponent()
        {
        
        }
        public Task<IViewComponentResult> InvokeAsync(
            TagListItemViewModel<Tag> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}
