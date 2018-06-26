using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Plato.Internal.Abstractions.Settings;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Entities.Controllers
{
    public class AdminController : Controller
    {

        private readonly IContextFacade _contextFacade;
        private readonly ISiteSettingsStore _settingsStore;

        private readonly IEntityRepository<Entity> _entityRepository;


        public AdminController(
            ISiteSettingsStore settingsStore, 
            IEntityRepository<Entity> entityRepository, IContextFacade contextFacade)
        {
            _settingsStore = settingsStore;
            _entityRepository = entityRepository;
            _contextFacade = contextFacade;
        }
        
        public async Task<IActionResult> Index()
        {

            var feature = await _contextFacade.GetCurrentFeatureAsync();

            ViewBag.Feature = feature;

            var entity = new Entity()
            {
                Title = "test",
                Markdown = "TExt"
            };

            await _entityRepository.InsertUpdateAsync(entity);

            return View();
        }

        

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }

    public class TextObject
    {
        public TextObject(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}
