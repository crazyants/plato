using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Modules.Simple
{
    [ViewComponent(Name = "Plato.Modules.Simple")]
    public class SimpleViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke(string value)
        {
            return View(value);
        }

    }
}
