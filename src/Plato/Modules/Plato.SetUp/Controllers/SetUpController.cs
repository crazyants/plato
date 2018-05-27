using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<SetUpController> _logger;
        private readonly IDataMigrationBuilder _dataMigrationBuilder;

        public SetUpController(
            ShellSettings shellSettings,
            ISiteSettingsStore settingsStore,
            ISetUpService setUpService,
            ISchemaLoader schemaLoader,
            IDataMigrationBuilder dataMigrationBuilder,
            ILogger<SetUpController> logger,
            IStringLocalizer<SetUpController> t)
        {
            _shellSettings = shellSettings;
            _settingsStore = settingsStore;
            _setUpService = setUpService;
            _schemaLoader = schemaLoader;
            _dataMigrationBuilder = dataMigrationBuilder;
            _logger = logger;
            T = t;
        }

        public IStringLocalizer T { get; set; }

        public async Task<IActionResult> Index()
        {

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Index action on SetUp controller invoked!");

            var setUpViewModel = new SetUpViewModel()
            {
                SiteName = "Plato",
                ConnectionString = "server=localhost;trusted_connection=true;database=Plato_Test2",
                TablePrefix = "plato",
                UserName = "admin",
                Email = "admin@admin.com",
                Password = "admin",
                PasswordConfirmation =  "admin"
            };

            return View(setUpViewModel);


        }

        [HttpPost]
        public async Task<IActionResult> Index(SetUpViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Index POST action on SetUp controller invoked!");
            
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
                AdminUsername = model.UserName,
                AdminEmail = model.Email,
                AdminPassword = model.Password,
                Errors = new Dictionary<string, string>()
            };
            
            if (!model.TablePrefixPreset)
            {
                setupContext.DatabaseTablePrefix = model.TablePrefix;
            }

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Beginning call to SetUpAsync");

            var executionId = await _setUpService.SetUpAsync(setupContext);

            // Check if a component in the Setup failed
            if (setupContext.Errors.Count > 0)
            {

                if (_logger.IsEnabled(LogLevel.Error))
                    _logger.LogInformation($"Set-up of tennet '{setupContext.SiteName}' failed with the following errors...");

                foreach (var error in setupContext.Errors)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation(error.Key + " " + error.Value);
                    ModelState.AddModelError(error.Key, error.Value);
                }
                return View(model);
            }

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Tennet with site name '{setupContext.SiteName}' created successfully");
            
            return Redirect("~/");
        }

    }

}
