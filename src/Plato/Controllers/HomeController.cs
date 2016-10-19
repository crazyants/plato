using Microsoft.AspNetCore.Mvc;
using Plato.Environment.Modules;
using Plato.Repositories;
using System.Collections.Generic;
using Plato.Environment.Modules.Abstractions;
using Plato.Models.Users;
using System.Threading.Tasks;

namespace Plato.Controllers
{
    public class HomeController : Controller
    {
            
        private readonly IModuleLocator _moduleLocator;
        private IUserRepository<User> _userRepository;

        public HomeController(
            IModuleLocator moduleLocator,
            IUserRepository<User> userRepository)
        {
            //_fileSystem = fileSystem;
            _moduleLocator = moduleLocator;
            _userRepository = userRepository;
            
        }

   
        public async Task<IActionResult> Index()
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


            System.Random rand = new System.Random();
            
            User newUser = await _userRepository.InsertUpdate(
                new User()
                {
                    UserName = "John Doe" + rand.Next(1, 500),
                    Email = "email" + +rand.Next(1, 500) + "@address.com",
                    DisplayName = "Jon Doe" + rand.Next(1, 500),
                    Detail = new UserDetail()
                    {
                        EditionId = 0,
                        FirstName = "Jonny",
                        LastName = "Doe",
                        RoleId = 5,
                        WebSiteUrl = "http://www.instantasp.co.uk/"
                    },
                    Secret = new UserSecret()
                    {
                        Password = "123",
                        Salts = new int[] { +rand.Next(1, 500), 123232 }
                    }
                });
            
                  
            //var user = _userRepository.SelectById(1);
                    

            return View(newUser);
            //return RedirectToAction("Index", "Discussions"); 
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
