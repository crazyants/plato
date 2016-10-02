using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Modules.Login
{

    [ViewComponent(Name = "Plato.Modules.Login")]
    public class LoginViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke(object value)
        {
            return View(value);
        }

    }
}
