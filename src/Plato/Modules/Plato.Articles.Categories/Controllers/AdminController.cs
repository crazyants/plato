using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Features;
using Plato.Internal.Navigation.Abstractions;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Articles.Categories.Models;
using Plato.Articles.Categories.ViewModels;
using Plato.Internal.Layout;

namespace Plato.Articles.Categories.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {
     
        private readonly IViewProviderManager<CategoryAdmin> _viewProvider;
        private readonly ICategoryManager<Category> _categoryManager;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IFeatureFacade _featureFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            ICategoryStore<Category> categoryStore,
            IViewProviderManager<CategoryAdmin> viewProvider,
            ICategoryManager<Category> categoryManager,
            IBreadCrumbManager breadCrumbManager,
            IFeatureFacade featureFacade,
            IAlerter alerter)
        {
            _breadCrumbManager = breadCrumbManager;
            _categoryManager = categoryManager;
            _featureFacade = featureFacade;
            _categoryStore = categoryStore;
            _viewProvider = viewProvider;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;
        }

        // --------------
        // Manage Categories
        // --------------

        public async Task<IActionResult> Index(int id)
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}
            
            IEnumerable<Category> parents = null;
            if (id > 0)
            {
                parents = await _categoryStore.GetParentsByIdAsync(id);
            }

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Articles"], articles => articles
                        .Action("Index", "Admin", "Plato.Articles")
                        .LocalNav());

                if (parents == null)
                {
                    builder.Add(S["Categories"]);
                }
                else
                {
                    builder.Add(S["Categories"], categories => categories
                        .Action("Index", "Admin", "Plato.Articles.Categories", new RouteValueDictionary { ["Id"] = 0 })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        if (parent.Id != id)
                        {
                            builder.Add(S[parent.Name], categories => categories
                                .Action("Index", "Admin", "Plato.Articles.Categories", new RouteValueDictionary { ["Id"] = parent.Id })
                                .LocalNav()
                            );
                        }
                        else
                        {
                            builder.Add(S[parent.Name]);
                        }
                     
                    }
                }

            });

            // Get optional current category
            Category currentCategory = null;
            if (id > 0)
            {
                currentCategory = await _categoryStore.GetByIdAsync(id);
            }
            
            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideIndexAsync(currentCategory ?? new Category(), this));

        }

        // --------------
        // Create Category
        // --------------

        public async Task<IActionResult> Create(int id = 0)
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Articles"], articles => articles
                        .Action("Index", "Admin", "Plato.Articles")
                        .LocalNav())
                    .Add(S["Categories"], categories => categories
                        .Action("Index", "Admin", "Plato.Articles.Categories")
                        .LocalNav())
                    .Add(S["Add Category"]);
            });
            
            // We need to pass along the featureId
            var feature = await GetCurrentFeature();
            var model = await _viewProvider.ProvideEditAsync(new CategoryAdmin
            {
                ParentId = id,
                FeatureId = feature.Id

            }, this);
            return View(model);

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditChannelViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                return await Create(viewModel.Id);
            }

            var iconCss = viewModel.IconCss;
            if (!string.IsNullOrEmpty(iconCss))
            {
                iconCss = viewModel.IconPrefix + iconCss;
            }

            var feature = await GetCurrentFeature();
            var category =  new Category()
            {
                ParentId = viewModel.ParentId,
                FeatureId = feature.Id,
                Name = viewModel.Name,
                Description = viewModel.Description,
                ForeColor = viewModel.ForeColor,
                BackColor = viewModel.BackColor,
                IconCss = iconCss
            };

            var result = await _categoryManager.CreateAsync(category);
            if (result.Succeeded)
            {

                await _viewProvider.ProvideUpdateAsync(result.Response, this);

                _alerter.Success(T["Category Added Successfully!"]);

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

        // --------------
        // Edit Category
        // --------------

        public async Task<IActionResult> Edit(int id)
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Articles"], articles => articles
                        .Action("Index", "Admin", "Plato.Articles")
                        .LocalNav())
                    .Add(S["Categories"], categories => categories
                        .Action("Index", "Admin", "Plato.Articles.Categories")
                        .LocalNav())
                    .Add(S["Edit Category"]);
            });
            
            var category = await _categoryStore.GetByIdAsync(id);
    
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(category, this));

        }
        
        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
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

            _alerter.Success(T["Category Updated Successfully!"]);

            return RedirectToAction(nameof(Index));

        }

        // --------------
        // Delete
        // --------------

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            
            var ok = int.TryParse(id, out int categoryId);
            if (!ok)
            {
                return NotFound();
            }

            // Get category
            var currentCategory = await _categoryStore.GetByIdAsync(categoryId);

            // Ensure category exists
            if (currentCategory == null)
            {
                return NotFound();
            }

            // Delete
            var result = await _categoryManager.DeleteAsync(currentCategory);

            if (result.Succeeded)
            {
                _alerter.Success(T["Category Deleted Successfully"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
         
            }

            return RedirectToAction(nameof(Index));
        }

        // --------------
        // Move Up / Down
        // --------------

        public async Task<IActionResult> MoveUp(int id)
        {

            var category = await _categoryStore.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var result = await _categoryManager.Move(category, MoveDirection.Up);
            if (result.Succeeded)
            {
                _alerter.Success(T["Category Updated Successfully"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> MoveDown(int id)
        {

            var category = await _categoryStore.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var result = await _categoryManager.Move(category, MoveDirection.Down);
            if (result.Succeeded)
            {
                _alerter.Success(T["Category Updated Successfully"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }

            return RedirectToAction(nameof(Index));

        }

        // ---------

        async Task<IShellFeature> GetCurrentFeature()
        {
            var featureId = "Plato.Articles.Categories";
            var feature = await _featureFacade.GetFeatureByIdAsync(featureId);
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the Id '{featureId}'");
            }
            return feature;
        }
        
    }

}
