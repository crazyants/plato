using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Ideas.Models;
using Plato.Entities.ViewModels;

namespace Plato.Ideas.ViewComponents
{
    public class IdeaListItemViewComponent : ViewComponent
    {
        
        public IdeaListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(
            EntityListItemViewModel<Idea> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

