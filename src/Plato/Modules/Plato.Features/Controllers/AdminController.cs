using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Features;
using Plato.Internal.Modules.Abstractions;
using Plato.Features.ViewModels;

namespace Plato.Features.Controllers
{

    public class AdminController : Controller
    {

        private readonly IModuleManager _moduleManager;
        private readonly IShellFeatureManager _shellFEatureManager;

        public AdminController(
            IModuleManager moduleManager, 
            IShellFeatureManager shellFEatureManager)
        {
            _moduleManager = moduleManager;
            _shellFEatureManager = shellFEatureManager;
        }
        
        public async Task<IActionResult> Index()
        {

            var modules = await _moduleManager.LoadModulesAsync();
            
            //var enabledFeatures = _shellFEatureManager.

            var model = new FEaturesViewModel()
            {
                Modules = modules
            };
            
            return View(model);



        }
    }
}
