using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Plato.Abstractions.Stores;
using Plato.Data.Abstractions.Schemas;
using Plato.SetUp.ViewModels;
using Plato.SetUp.Services;

namespace Plato.SetUp.Controllers
{
    public class SetUpController : Controller
    {

        private readonly ISiteSettingsStore _settingsStore;
        private readonly ISetUpService _setUpService;

        private readonly ISchemaLoader _schemaLoader;

        public SetUpController(
            ISiteSettingsStore settingsStore,
            ISetUpService setUpService,
            ISchemaLoader schemaLoader,
            IStringLocalizer<SetUpController> t)
        {
            _settingsStore = settingsStore;
            _setUpService = setUpService;
            _schemaLoader = schemaLoader;

            T = t;
        }

        public IStringLocalizer T { get; set; }

        public IActionResult Index()
        {

            _schemaLoader.LoadSchemas(new List<string> {"1.0.0"});

            ViewData["Schema"] = _schemaLoader.LoadedSchemas.Count;



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
