using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Docs.Tags.Models;
using Plato.Tags.ViewModels;

namespace Plato.Docs.Tags.ViewComponents
{

    public class DocsTagListItemViewComponent : ViewComponent
    {

   
        public DocsTagListItemViewComponent()
        {
        
        }
        public Task<IViewComponentResult> InvokeAsync(
            TagListItemViewModel<Tag> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}
