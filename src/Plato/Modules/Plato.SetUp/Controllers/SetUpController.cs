using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Plato.Abstractions.Stores;
using Plato.SetUp.ViewModels;
using Plato.SetUp.Services;

namespace Plato.SetUp.Controllers
{
    public class SetUpController : Controller
    {

        private readonly ISiteSettingsStore _settingsStore;
        private readonly ISetUpService _setUpService;

        public SetUpController(
            ISiteSettingsStore settingsStore,
            ISetUpService setUpService,
            IStringLocalizer<SetUpController> t)
        {
            _settingsStore = settingsStore;
            _setUpService = setUpService;
            T = t;
        }

        public IStringLocalizer T { get; set; }

        public IActionResult Index()
        {
      
            return View();

        }

        [HttpPost]
        public IActionResult Index(SetUpViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var context = new SetUpContext()
            {
                SiteName = model.SiteName,
                DatabaseProvider = "SqlClient",
                DatabaseConnectionString = "server=localhost;trusted_connection=true;database=plato_2"
            };

            _setUpService.SetupAsync(context);

            return Redirect("~/");
        }

    }

}
