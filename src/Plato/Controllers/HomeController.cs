using Microsoft.AspNetCore.Mvc;
using Plato.Environment.Modules;
using Plato.Repositories;
using System.Collections.Generic;
using Plato.Environment.Modules.Abstractions;
using Plato.Models.Users;

using Plato.Repositories.Settings;
using Plato.Models.Settings;

using System.Threading.Tasks;
using Plato.Shell;

namespace Plato.Controllers
{
    public class HomeController : Controller
    {
            
        private readonly IModuleLocator _moduleLocator;
        private IUserRepository<User> _userRepository;
        private ISettingRepository<Setting> _settingRepository;
        private ISettingsFactory _settingsFactory;
        private IShellSettingsManager _shellSettingsManager;

        public HomeController(
            IModuleLocator moduleLocator,
            IUserRepository<User> userRepository,
            ISettingRepository<Setting> settingRepository,
            ISettingsFactory settingsFactory,
            IShellSettingsManager shellSettingsManager)
        {
            //_fileSystem = fileSystem;
            _moduleLocator = moduleLocator;
            _userRepository = userRepository;
            _settingRepository = settingRepository;
            _settingsFactory = settingsFactory;
            _shellSettingsManager = shellSettingsManager;



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
            System.Text.StringBuilder sb = new System.Text.StringBuilder();


            IEnumerable<ShellSettings> shellSettings =
                _shellSettingsManager.LoadSettings();

            foreach(var shellSetting in shellSettings)
            {
                sb.Append(shellSetting.Name);
                sb.Append(" - ");
                sb.Append(shellSetting.ConnectionString);
                sb.Append(" - ");
                sb.Append(shellSetting.RequestUrlPrefix);
                sb.Append(" - ");
                sb.Append(shellSetting.RequestUrlHost);
                sb.Append(" - ");
                sb.Append("<br><br>");

            }



         
            // ------------------------
            // settings 
            // -------------------------

        
            var newSetting = await _settingRepository.InsertUpdate(
                new Setting()
                {
                    SiteId = 1,
                    SpaceId = 0,
                    Key = "Group 2",
                    Value = "{ 'Group 1': 123, 'Group 2': 'hello', 'Group 3': true }"
                });


            var sf = _settingsFactory.SelectBySiteId(1);
            var settings = sf.Result.Settings;


            var test = _settingsFactory.TryGetValue("Group 4");

            foreach (var setting in settings)
            {
                sb.Append(setting.Key + " : " + setting.Value);
                sb.Append("<br>");
            }

   

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


            ViewData["result"] = sb.ToString();

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
