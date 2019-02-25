using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Plato.Internal.Abstractions.Settings;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Shell.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Articles.Controllers
{
    public class AdminController : Controller
    {
        private readonly IContextFacade _contextFacade;
        private readonly ISiteSettingsStore _settingsStore;
   
        public AdminController(
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade)
        {
            _settingsStore = settingsStore;
            _contextFacade = contextFacade;
        }
        
        public Task<IActionResult> Index()
        {
            return Task.FromResult((IActionResult)View());
        }

        

    }

}
