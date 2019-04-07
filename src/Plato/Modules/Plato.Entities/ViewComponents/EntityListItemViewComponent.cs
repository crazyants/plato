using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Models;
using Plato.Entities.ViewModels;

namespace Plato.Entities.ViewComponents
{
    public class EntityListItemViewComponent : ViewComponent
    {

        public EntityListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(EntityListItemViewModel<Entity> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));

        }

    }
    
}

