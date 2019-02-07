using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewComponents
{

    public class UserListItemViewComponent : ViewComponent
    {
        
        public UserListItemViewComponent()
        {

        }
        public Task<IViewComponentResult> InvokeAsync(
            UserListItemViewModel model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}
