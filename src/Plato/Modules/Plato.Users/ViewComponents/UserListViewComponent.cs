using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
