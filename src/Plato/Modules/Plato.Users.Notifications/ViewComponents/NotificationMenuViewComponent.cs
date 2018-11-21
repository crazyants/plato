using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Users.Notifications.ViewComponents
{
  
    public class NotificationMenuViewComponent : ViewComponent
    {

        private readonly IContextFacade _contextFacade;
      
        public NotificationMenuViewComponent(IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _contextFacade.GetAuthenticatedUserAsync());
        }

    }

}
