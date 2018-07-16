﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Discuss.Channels.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {
        private readonly IContextFacade _contextFacade;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly ICategoryManager<Category> _categoryManager;
        private readonly IViewProviderManager<Category> _viewProvider;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            ICategoryStore<Category> categoryStore,
            IViewProviderManager<Category> viewProvider,
            IBreadCrumbManager breadCrumbManager,
            IAlerter alerter, 
            ICategoryManager<Category> categoryManager)
        {
            _settingsStore = settingsStore;
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
            _viewProvider = viewProvider;
            _alerter = alerter;
            _categoryManager = categoryManager;
            _breadCrumbManager = breadCrumbManager;

            T = htmlLocalizer;
            S = stringLocalizer;
        }

        public async Task<IActionResult> Index()
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}
            
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Channels"]);
            });
            
            var model = await _viewProvider.ProvideIndexAsync(new Category(), this);
            return View(model);
        }

        public async Task<IActionResult> Create()
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Channels"], channels => channels
                    .Action("Index", "Admin", "Plato.Discuss.Channels")
                    .LocalNav()
                ).Add(S["Add Channel"]);
            });


            var model = await _viewProvider.ProvideEditAsync(new Category(), this);
            return View(model);
        }

        [HttpPost]
        [ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditChannelViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var featureId = 0;
            var feature = await _contextFacade.GetFeatureByAreaAsync();
            if (feature != null)
            {
                featureId = feature.Id;
            }

            var category =  new Category()
            {
                FeatureId = featureId,
                Name = viewModel.Name,
                Description = viewModel.Description,
                ForeColor = viewModel.ForeColor,
                BackColor = viewModel.BackColor,
                IconCss = viewModel.IconPrefix + viewModel.IconCss
            };

            var result = await _categoryManager.CreateAsync(category);
            if (result.Succeeded)
            {

                await _viewProvider.ProvideUpdateAsync(result.Response, this);

                _alerter.Success(T["Channel Added Successfully!"]);

                return RedirectToAction(nameof(Index));

            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(viewModel);


        }
        
        public async Task<IActionResult> Edit(int id)
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Channels"], channels => channels
                    .Action("Index", "Admin", "Plato.Discuss.Channels")
                    .LocalNav()
                ).Add(S["Edit Channel"]);
            });
            
            var category = await _categoryStore.GetByIdAsync(id);
            var model = await _viewProvider.ProvideEditAsync(category, this);
            return View(model);

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

            _alerter.Success(T["Channel Updated Successfully!"]);

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            
            var ok = int.TryParse(id, out int categoryId);
            if (!ok)
            {
                return NotFound();
            }

            var currentCategory = await _categoryStore.GetByIdAsync(categoryId);

            if (currentCategory == null)
            {
                return NotFound();
            }

            var success = await _categoryStore.DeleteAsync(currentCategory);

            if (success)
            {
                _alerter.Success(T["Channel Deleted Successfully"]);
            }
            else
            {

                _alerter.Danger(T["Could not delete the channel"]);
            }

            return RedirectToAction(nameof(Index));
        }


    }

}