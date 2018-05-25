using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Plato.Abstractions.SetUp;
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
                DatabaseConnectionString = model.ConnectionString,
                Errors = new Dictionary<string, string>()
            };
            
            if (!model.TablePrefixPreset)
            {
                setupContext.DatabaseTablePrefix = model.TablePrefix;
            }
            
            var executionId = _setUpService.SetUpAsync(setupContext);

            // Check if a component in the Setup failed
            if (setupContext.Errors.Count > 0)
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
