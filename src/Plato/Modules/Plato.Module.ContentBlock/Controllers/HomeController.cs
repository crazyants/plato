using Microsoft.AspNetCore.Mvc;

namespace Plato.Modules.ContentBlock.Controllers
{
    public class HomeController : Controller
    {
        
              
        public IActionResult Index()
        {
        
            ViewData["result"] = "Hellow from content block";

            return View();
        }


    }
}
