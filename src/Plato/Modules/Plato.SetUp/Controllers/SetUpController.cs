using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Plato.Abstractions.Stores;
using Plato.Data.Abstractions.Schemas;
using Plato.Data.Migrations;
using Plato.SetUp.ViewModels;
using Plato.SetUp.Services;
using Plato.Shell.Models;

namespace Plato.SetUp.Controllers
{
    public class SetUpController : Controller
    {

        private readonly ShellSettings _shellSettings;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly ISetUpService _setUpService;
        private readonly ISchemaLoader _schemaLoader;
        private readonly IDataMigrationBuilder _dataMigrationBuilder;

        public SetUpController(
            ShellSettings shellSettings,
            ISiteSettingsStore settingsStore,
            ISetUpService setUpService,
            ISchemaLoader schemaLoader,
            IDataMigrationBuilder dataMigrationBuilder,
            IStringLocalizer<SetUpController> t)
        {
            _shellSettings = shellSettings;
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


            var setUpViewModel = new SetUpViewModel()
            {
                SiteName = "",
                ConnectionString = ""
            };

            return View(setUpViewModel);


        }

        [HttpPost]
        public IActionResult Index(SetUpViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            if (!string.IsNullOrEmpty(_shellSettings.ConnectionString))
            {
                model.ConnectionStringPreset = true;
                model.ConnectionString = _shellSettings.ConnectionString;
            }

            if (!string.IsNullOrEmpty(_shellSettings.TablePrefix))
            {
                model.TablePrefixPreset = true;
                model.TablePrefix = _shellSettings.TablePrefix;
            }

            var setupContext = new SetUpContext()
            {
                SiteName = model.SiteName,
                DatabaseProvider = "SqlClient",
                DatabaseConnectionString = model.ConnectionString
            };


            if (!model.TablePrefixPreset)
            {
                setupContext.DatabaseTablePrefix = model.TablePrefix;
            }


            var executionId = _setUpService.SetupAsync(setupContext);
            // Check if a component in the Setup failed
            if (setupContext.Errors.Any())
            {
                foreach (var error in setupContext.Errors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }

                return View(model);
            }

            return Redirect("~/");
        }

    }

}
