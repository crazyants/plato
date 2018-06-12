using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Modules.Abstractions;
using Plato.Modules.ViewModels;

namespace Plato.Modules.Controllers
{

    public class AdminController : Controller
    {

        private readonly IModuleManager _moduleManager;

        public AdminController(IModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
        }
        
        public async Task<IActionResult> Index()
        {

            var modules = await _moduleManager.LoadModulesAsync();
            
            var model = new ModulesViewModel()
            {
                Modules = modules
            };
            
            return View(model);



        }
    }
}
