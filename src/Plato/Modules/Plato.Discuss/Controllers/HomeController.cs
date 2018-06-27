using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Plato.Internal.Abstractions.Settings;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Discuss.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContextFacade _contextFacade;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly IEntityRepository<Entity> _entityRepository;

        public HomeController(
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            IEntityRepository<Entity> entityRepository)
        {
            _settingsStore = settingsStore;
            _contextFacade = contextFacade;
            _entityRepository = entityRepository;
        }
        
        public async Task<IActionResult> Index()
        {

            string path = Request.Path;
            ViewData["path"] = path;

            var feature = await _contextFacade.GetCurrentFeatureAsync();

            ViewBag.Feature = feature;

            var entity = new Entity()
            {
                FeatureId = feature.Id,
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
