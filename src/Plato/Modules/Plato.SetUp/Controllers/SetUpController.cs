using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Plato.Abstractions.Stores;
using Plato.SetUp.ViewModels;

namespace Plato.SetUp
{
    public class SetUpController : Controller
    {

        private readonly ISiteSettingsStore _settingsStore;

        public SetUpController(
            ISiteSettingsStore settingsStore,
            IStringLocalizer<SetUpController> t)
        {
            _settingsStore = settingsStore;
            T = t;
        }

        public IStringLocalizer T { get; set; }

        public async Task<IActionResult> Index()
        {

            var model = new SetUpViewModel();
            model.SiteName = "Example Site Name";

            return View(model);

        }

    }

}
