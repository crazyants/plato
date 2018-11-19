using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Users.Notifications.ViewComponents
{
  
    public class NotificationMenuViewComponent : ViewComponent
    {
  
      
        public NotificationMenuViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult((IViewComponentResult) View());
        }

    }

}
