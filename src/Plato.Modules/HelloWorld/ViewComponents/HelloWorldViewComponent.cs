using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Modules.HelloWorld
{

    [ViewComponent(Name = "Plato.Modules.HelloWorld")]
    public class HelloWorldViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke(string value)
        {
            return View((object)"Hello " + value);
        }

    }
}
