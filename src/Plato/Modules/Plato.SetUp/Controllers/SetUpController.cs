using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Plato.Abstractions.Stores;
using Plato.Data.Abstractions.Schemas;
using Plato.Data.Migrations;
using Plato.SetUp.ViewModels;
using Plato.SetUp.Services;

namespace Plato.SetUp.Controllers
{
    public class SetUpController : Controller
    {

        private readonly ISiteSettingsStore _settingsStore;
        private readonly ISetUpService _setUpService;

        private readonly ISchemaLoader _schemaLoader;
        private readonly IDataMigrationBuilder _dataMigrationBuilder;

        public SetUpController(
            ISiteSettingsStore settingsStore,
            ISetUpService setUpService,
            ISchemaLoader schemaLoader,
            IDataMigrationBuilder dataMigrationBuilder,
            IStringLocalizer<SetUpController> t)
        {
            _settingsStore = settingsStore;
            _setUpService = setUpService;

            _schemaLoader = schemaLoader;
            _dataMigrationBuilder = dataMigrationBuilder;

            T = t;
        }

        public IStringLocalizer T { get; set; }

        public async Task<IActionResult> Index()
        {

            //_schemaLoader.LoadSchemas(new List<string> {"1.0.0"});
            
            //ViewData["Schema"] = _schemaLoader.LoadedSchemas.Count;
            //ViewData["Schema"] = _schemaLoader.LoadedSchemas[0].InstallSql;


            //_dataMigrationBuilder.BuildMigrations(new List<string>() {"1.0.0"});

            _dataMigrationBuilder.BuildMigrations(new List<string>() { "1.0.0" });
            ViewData["Schema"] = _dataMigrationBuilder.DataMigrationType.ToString();

            //_dataMigrationBuilder.BuildMigrations(new List<string>() { "1.0.1", "1.0.0" });

            //_dataMigrationBuilder.BuildMigrations(new List<string>()
            //{
            //    "1.0.1",
            //    "1.0.2",
            //    "1.0.3",
            //    "1.0.4"
            //});

            //_dataMigrationBuilder.BuildMigrations(new List<string>()
            //{
            //    "1.0.4",
            //    "1.0.3",
            //    "1.0.2",
            //    "1.0.1"
            //});

            //_dataMigrationBuilder.BuildMigrations(new List<string>()
            //{
            //    "1.0.0",
            //    "2.0."
            //});

            //_dataMigrationBuilder.BuildMigrations(new List<string>()
            //{
            //    "2.0.0",
            //    "1.0.0"
            //});

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
