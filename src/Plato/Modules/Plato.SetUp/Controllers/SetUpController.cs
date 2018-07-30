using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.SetUp.ViewModels;
using Plato.SetUp.Services;

namespace Plato.SetUp.Controllers
{
    public class SetUpController : Controller
    {

        private readonly IShellSettings _shellSettings;
  
        private readonly ISetUpService _setUpService;
     
        private readonly ILogger<SetUpController> _logger;
     
        public SetUpController(
            IShellSettings shellSettings,
            ISetUpService setUpService,
            ILogger<SetUpController> logger,
            IStringLocalizer<SetUpController> t)
        {
            _shellSettings = shellSettings;
            _setUpService = setUpService;
            _logger = logger;
            T = t;
        }

        public IStringLocalizer T { get; set; }

        public IActionResult Index()
        {

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Index action on SetUp controller invoked!");

            var setUpViewModel = new SetUpViewModel()
            {
                SiteName = "Plato",
                ConnectionString = "server=localhost;trusted_connection=true;database=Plato_Test7",
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
