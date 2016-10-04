using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Abstractions.Layout;

namespace Plato.Modules.ContentArea
{
   
    [ViewComponent(Name = "Plato.Modules.ContentArea")]
    public class ContentAreaViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke(object value)
        {
            return View(value);
        }

    }
}
