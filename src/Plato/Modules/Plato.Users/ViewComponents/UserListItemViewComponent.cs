using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Navigation;
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
