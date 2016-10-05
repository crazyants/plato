using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Modules.Template
{

    [ViewComponent(Name = "Plato.Modules.Template")]
    public class TemplateViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke(dynamic value)
        {
            return View(value);
        }

    }
}
