using Microsoft.AspNetCore.Mvc;
using Plato.Login.Models;
using System.Threading.Tasks;

namespace Plato.Login
{
    public class LoginController : Controller
    {
                   
        public LoginController()
        {                      
        }
        
        public IActionResult Index()
        {

            string path = Request.Path;
            ViewData["path"] = path;

            var model = new LoginViewModel();
            model.Email = "123456";
            model.UserName = "admin";

            return View(model);

        }

        
    }
}
