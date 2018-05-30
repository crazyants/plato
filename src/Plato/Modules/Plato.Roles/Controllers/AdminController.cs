using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Roles.Controllers
{
    public class AdminController : Controller
    {

        public AdminController()
        {

        }

        [Route("{area:exists}/{controller}/{action}/{id?}")]
        public async Task<ActionResult> Index()
        {

            return View();
        }

    }
}
