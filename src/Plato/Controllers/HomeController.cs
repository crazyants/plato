using Microsoft.AspNetCore.Mvc;
using Plato.FileSystem;
using Plato.Environment.Modules;
using Plato.Repositories;
using Plato.Repositories.Models;
using System.Collections.Generic;

namespace Plato.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly IPlatoFileSystem _fileSystem;
        private readonly IModuleLocator _moduleLocator;
        private IUserRepository<User> _userRepository;

        public HomeController(
            IPlatoFileSystem fileSystem,
            IModuleLocator moduleLocator,
            IUserRepository<User> userRepository)
        {
            _fileSystem = fileSystem;
            _moduleLocator = moduleLocator;
            _userRepository = userRepository;
            
        }
        
        public IActionResult Index()
        {

            string path = Request.Path;
            ViewData["path"] = path;

            string rootDirectory = _fileSystem.GetDirectoryInfo("Modules").FullName;

            //var result = _moduleLocator.LocateModules(
            //    new string[] {
            //        rootDirectory
            //    }, 
            //    "Module", 
            //    "module.txt", 
            //    false);                

            //ViewData["result"] = result;
                  
            var user = _userRepository.Select(1);

            var users = new List<User>();
            users.Add(user);

            return View(users);
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
