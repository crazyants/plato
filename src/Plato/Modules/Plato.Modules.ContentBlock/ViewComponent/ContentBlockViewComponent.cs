using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Modules.ContentBlock
{

    [ViewComponent(Name = "Plato.Modules.ContentBlock")]
    public class ContentBlockViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke(object value)
        {
            return View(value);
        }

    }
}
