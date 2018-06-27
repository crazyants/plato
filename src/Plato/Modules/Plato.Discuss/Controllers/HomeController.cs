using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Plato.Discuss.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Discuss.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContextFacade _contextFacade;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly IEntityRepository<Entity> _entityRepository;
        private readonly IEntityStore<Entity> _entityStore;

        public HomeController(
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            IEntityRepository<Entity> entityRepository,
            IEntityStore<Entity> entityStore)
        {
            _settingsStore = settingsStore;
            _contextFacade = contextFacade;
            _entityRepository = entityRepository;
            _entityStore = entityStore;
        }
        
        public async Task<IActionResult> Index()
        {

            string path = Request.Path;
            ViewData["path"] = path;

            var feature = await _contextFacade.GetCurrentFeatureAsync();

            ViewBag.Feature = feature;
            
            var rnd = new Random();
            var entity = new Entity()
            {
                FeatureId = feature.Id,
                Title = "Test Topic " + rnd.Next(0, 100000).ToString(),
                Markdown = "Test message " + rnd.Next(0, 100000).ToString(),
                Html = "Test message " + rnd.Next(0, 100000).ToString(),
                PlainText = "Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message  " + rnd.Next(0, 100000).ToString()
            };

            var newEntity = await _entityStore.CreateAsync(entity);

            var pagerOptions = new PagerOptions()
            {
                Page = 1,
                PageSize = 20
            };

            var model = new DiscussIndexViewModel()
            {
                Results = await GetEntities(pagerOptions)
            };
            
            return View(model);

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

        public async Task<IPagedResults<Entity>> GetEntities(PagerOptions pagerOptions)
        {
            return await _entityStore.QueryAsync()
                .Page(pagerOptions.Page, pagerOptions.PageSize)
                .Select<EntityQueryParams>(q =>
                {
                    //if (!string.IsNullOrEmpty(filterOptions.Search))
                    //{
                    //    q.UserName.IsIn(filterOptions.Search).Or();
                    //    q.Email.IsIn(filterOptions.Search);
                    //}
                    // q.UserName.IsIn("Admin,Mark").Or();
                    // q.Email.IsIn("email440@address.com,email420@address.com");
                    // q.Id.Between(1, 5);
                })
                .OrderBy("Id", OrderBy.Desc)
                .ToList<Entity>();
        }




    }



}
