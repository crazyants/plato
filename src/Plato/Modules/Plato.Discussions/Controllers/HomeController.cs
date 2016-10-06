using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Plato.Discussions
{
    public class DiscussionsController : Controller
    {
                   
        public DiscussionsController()
        {                      
        }
        
        public IActionResult Index()
        {

            string path = Request.Path;
            ViewData["path"] = path;

            //string rootDirectory = _fileSystem.GetDirectoryInfo("Modules").FullName;

            //var result = _moduleLocator.LocateModules(
            //    new string[] {
            //        rootDirectory
            //    }, 
            //    "Module", 
            //    "module.txt", 
            //    false);                

            //ViewData["result"] = result;
                  
         
            return View();
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
