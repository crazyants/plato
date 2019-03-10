using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Models;
using Plato.Entities.ViewModels;

namespace Plato.Search.ViewComponents
{
    public class SearchListItemViewComponent : ViewComponent
    {

        public SearchListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(EntityListItemViewModel<Entity> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));

        }

    }
    
}

