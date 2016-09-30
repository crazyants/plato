using Microsoft.AspNetCore.Mvc;
using Plato.FileSystem;
using Plato.Environment.Modules;

namespace Plato.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly IPlatoFileSystem _fileSystem;
        private readonly IModuleLocator _moduleLocator;

        public HomeController(
            IPlatoFileSystem fileSystem,
            IModuleLocator moduleLocator)
        {
            _fileSystem = fileSystem;
            _moduleLocator = moduleLocator;
            
        }
        
        public IActionResult Index()
        {

            string path = Request.Path;
            ViewData["path"] = path;

            string rootDirectory = _fileSystem.GetDirectoryInfo("Modules").FullName;

            var result = _moduleLocator.LocateModules(
                new string[] {
                    rootDirectory
                }, 
                "Module", 
                "component.txt", 
                false);                

            ViewData["result"] = result;

            return View(result);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
