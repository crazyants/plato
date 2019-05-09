using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.ViewModels;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Entities.ViewComponents
{
  
    public class EntityIsPrivateDropDownViewComponent : ViewComponent
    {
  
        public EntityIsPrivateDropDownViewComponent(IContextFacade contextFacade)
        {
        }

        public Task<IViewComponentResult> InvokeAsync(EntityIsPrivateDropDownViewModel model)
        {

            if (model == null)
            {
                model = new EntityIsPrivateDropDownViewModel();
            }

            return Task.FromResult((IViewComponentResult) View(model));

        }

    }

}
