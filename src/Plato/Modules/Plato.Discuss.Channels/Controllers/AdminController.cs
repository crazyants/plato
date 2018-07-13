using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Discuss.Channels.Controllers
{
    public class AdminController : Controller
    {
        private readonly IContextFacade _contextFacade;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly ICategoryStore<Category> _categoryStore;

        public AdminController(
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            ICategoryStore<Category> categoryStore)
        {
            _settingsStore = settingsStore;
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
        }
        
        public async Task<IActionResult> Index()
        {

            string path = Request.Path;
            ViewData["path"] = path;

            var feature = await _contextFacade.GetFeatureByAreaAsync();

            ViewBag.Feature = feature;

            var category = new Category()
            {
                FeatureId = feature.Id,
                Name = "test",
                IconCss = "fal fa-plane"
            };

            var detail = category.GetOrCreate<ChannelDetails>();
            detail.TotalTopics = 100;

            category.AddOrUpdate<ChannelDetails>(detail);


            await _categoryStore.CreateAsync(category);



            var model = await GetModel();


            return View(model);
        }


        async Task<ChannelsViewModel> GetModel()
        {

            var featureId = "Plato.Discuss.Channels";
            var feature = await _contextFacade.GetFeatureByModuleIdAsync(featureId);
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the Id '{featureId}'");
            }

            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);

            return new ChannelsViewModel()
            {
                Channels = categories,
                EditChannel = new Category(),
                ChannelIcons = new DefaultIcons()
            };

        }


    }

}
