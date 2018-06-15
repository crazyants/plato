using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Features;
using Plato.Internal.Modules.Abstractions;
using Plato.Features.ViewModels;
using Plato.Internal.Models.Shell;

namespace Plato.Features.Controllers
{

    public class AdminController : Controller
    {
        
        private readonly IShellFeatureManager _shellFeatureManager;
        private readonly IShellDescriptorFeatureManager _shellDescriptorFeatureManager;

        public AdminController(
            IShellFeatureManager shellFeatureManager,
            IShellDescriptorFeatureManager shellDescriptorFeatureManager)
        {
            _shellFeatureManager = shellFeatureManager;
            _shellDescriptorFeatureManager = shellDescriptorFeatureManager;
        }
        
        public async Task<IActionResult> Index()
        {

            var features = await _shellDescriptorFeatureManager.GetFeaturesAsync();


              //var enabledFeatures = _shellFEatureManager.

            var model = new FeaturesViewModel()
            {
                Features = features
            };
            
            return View(model);
            
        }

        [HttpPost]
        public async Task<IActionResult> Enable(string id)
        {

            var test = "test";

            var results = await _shellFeatureManager.EnableFeaturesAsync(new string[] {id});

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public async Task<IActionResult> Disable(string id)
        {

            var test = "test";

            var results = await _shellFeatureManager.EnableFeaturesAsync(new string[] { id });

            return RedirectToAction(nameof(Index));

        }




    }
}
