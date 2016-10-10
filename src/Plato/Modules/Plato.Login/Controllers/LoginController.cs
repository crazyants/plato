using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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

            //string rootDirectory = _fileSystem.GetDirectoryInfo("Modules").FullName;

            //var result = _moduleLocator.LocateModules(
            //    new string[] {
            //        rootDirectory
            //    }, 
            //    "Module", 
            //    "module.txt", 
            //    false);                

            //ViewData["result"] = result;

            List<TextObject1> list = new List<TextObject1>();
            list.Add(new TextObject1("Ryan"));
            list.Add(new TextObject1("Jane"));
            list.Add(new TextObject1("Mike"));
            list.Add(new TextObject1("Roger"));

            return View(list);
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

    public class TextObject1
    {
        public TextObject1(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}
