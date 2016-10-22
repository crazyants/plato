using Microsoft.AspNetCore.Mvc;
using Plato.Modules;
using Plato.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Modules.Abstractions;
using Plato.Models.Users;
using Plato.Shell.Models;
using Plato.Repositories.Settings;
using Plato.Models.Settings;
using Plato.Shell;
using Plato.Shell.Extensions;


using Plato.Abstractions.Settings;
using Microsoft.AspNetCore.Routing;

namespace Plato.Controllers
{
    public class HomeController : Controller
    {
            
        private readonly IModuleLocator _moduleLocator;
        private IUserRepository<User> _userRepository;
        private ISettingRepository<Setting> _settingRepository;
        private ISettingsFactory _settingsFactory;
        private IShellSettingsManager _shellSettingsManager;

        private IRunningShellTable _runningShellTable;

        public HomeController(
            IModuleLocator moduleLocator,
            IUserRepository<User> userRepository,
            ISettingRepository<Setting> settingRepository,
            ISettingsFactory settingsFactory,
            IShellSettingsManager shellSettingsManager,
            IRunningShellTable runningShellTable)
        {
            //_fileSystem = fileSystem;
            _moduleLocator = moduleLocator;
            _userRepository = userRepository;
            _settingRepository = settingRepository;
            _settingsFactory = settingsFactory;
            _shellSettingsManager = shellSettingsManager;
            _runningShellTable = runningShellTable;

            
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
                sb.Append("<br>");
                sb.Append(shellSetting.Location);
                sb.Append("<br>");
                sb.Append(shellSetting.ConnectionString);
                sb.Append("<br>");
                sb.Append(shellSetting.TablePrefix);
                sb.Append("<br>");
                sb.Append(shellSetting.RequestedUrlPrefix);
                sb.Append("<br>");
                sb.Append(shellSetting.RequestedUrlHost);        
                sb.Append("<br><br>");

            }

            sb.Append("Running Shell Table");
            sb.Append("<br>");

            var shellsByHostAndPrefi = _runningShellTable.ShellsByHostAndPrefix;

            foreach (var item in shellsByHostAndPrefi)
            {
                sb.Append("Key");
                sb.Append(item.Key);
                sb.Append("<br>");
                sb.Append("Value");
                sb.Append(item.Value.Name + " - ");
                sb.Append(item.Value.State.ToString()); ;
                sb.Append("<br>");
            }


            sb.Append("<br>");
            sb.Append("----------------------");
            sb.Append("<br>");


            var currentSettings = _runningShellTable.Match(HttpContext);

            sb.Append("Matched Shell Settings");
            sb.Append("<br>");

            sb.Append(currentSettings.ConnectionString);
            sb.Append("<br>");

            sb.Append(currentSettings.Name);


            sb.Append("<br>");
            sb.Append("----------------------");
            sb.Append("<br>");


            // ------------------------
            // settings 
            // -------------------------


           var HomeRoute = new RouteValueDictionary();
            HomeRoute.Add("Action", "Index");
            HomeRoute.Add("Controller", "Home");
            HomeRoute.Add("Area", "Orchard.Demo");

            SiteSettings settings =
                await _settingsFactory.UpdateSettingsAsync<SiteSettings>(SettingGroups.SiteSettings,
                new SiteSettings()
                {
                    PageTitleSeparator = "-",
                    BaseUrl = "1231231231313123123",
                    HomeRoute = HomeRoute
                });


            sb.Append("<br>");

            var test = _settingsFactory.GetSettingsAsync<SiteSettings>(SettingGroups.SiteSettings);

            if (test != null)
            {
                sb.Append("BASE URL: ");
                sb.Append(test.Result.BaseUrl);
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
