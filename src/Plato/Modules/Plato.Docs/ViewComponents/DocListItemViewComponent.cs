using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Docs.Models;
using Plato.Entities.ViewModels;

namespace Plato.Docs.ViewComponents
{
    public class DocListItemViewComponent : ViewComponent
    {
  
        public DocListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(EntityListItemViewModel<Doc> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}



