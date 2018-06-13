using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Features.Controllers
{

    public class ModulesController : Controller
    {
        public ModulesController()
        {

        }
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
