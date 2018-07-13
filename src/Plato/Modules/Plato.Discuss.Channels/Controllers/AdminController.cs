using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewModels;
using Plato.Discuss.Channels.ViewProviders;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Discuss.Channels.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {
        private readonly IContextFacade _contextFacade;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IViewProviderManager<Category> _viewProvider;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public AdminController(
            IHtmlLocalizer<AdminController> localizer,
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            ICategoryStore<Category> categoryStore,
            IViewProviderManager<Category> viewProvider,
            IAlerter alerter)
        {
            _settingsStore = settingsStore;
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
            _viewProvider = viewProvider;
            _alerter = alerter;

            T = localizer;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _viewProvider.ProvideIndexAsync(new Category(), this);
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = await _viewProvider.ProvideEditAsync(new Category(), this);
            return View(model);
        }
        
        public Task<IActionResult> Edit(string id)
        {
            return Task.FromResult((IActionResult)View());
        }

        [HttpPost]
        [ActionName(nameof(Edit))]
        public  async Task<IActionResult> EditPost(int id)
        {

            var currentCategory = await _categoryStore.GetByIdAsync(id);
            if (currentCategory == null)
            {
                return NotFound();
            }

            var result = await _viewProvider.ProvideUpdateAsync(currentCategory, this);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            _alerter.Success(T["User Updated Successfully!"]);

            return RedirectToAction(nameof(Index));

        }


     

    }

}
