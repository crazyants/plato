using Microsoft.AspNetCore.Mvc;

namespace Plato.Admin.Controllers
{

    public class AdminController : Controller
    {
        public AdminController()
        {

        }
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
