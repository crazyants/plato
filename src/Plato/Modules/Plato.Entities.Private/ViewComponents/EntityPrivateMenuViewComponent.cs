using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Private.ViewModels;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Entities.Private.ViewComponents
{
  
    public class EntityPrivateMenuViewComponent : ViewComponent
    {
  
        public EntityPrivateMenuViewComponent(IContextFacade contextFacade)
        {
        }

        public Task<IViewComponentResult> InvokeAsync(PrivateMenuViewModel model)
        {

            if (model == null)
            {
                model = new PrivateMenuViewModel();
            }

            return Task.FromResult((IViewComponentResult) View(model));

        }

    }

}
