using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Abstractions.Data;
using Plato.Abstractions.Query;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewComponents
{

    public class UserListViewComponent : ViewComponent
    {
        
        public async Task<IViewComponentResult> InvokeAsync(string contentItemId = null, string displayType = null)
        {
            return View(null);
        }
        
    }
}
