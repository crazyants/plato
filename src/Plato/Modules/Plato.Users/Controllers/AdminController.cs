using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Users.Controllers
{

    public class AdminController : Controller
    {

        public AdminController()
        {
        }

        public async Task<ActionResult> Index()
        {
            return View();
        }

    }
}
