using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Features;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Discuss.Channels.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {
        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<Channel> _categoryStore;
        private readonly ICategoryManager<Channel> _categoryManager;
        private readonly IViewProviderManager<CategoryBase> _viewProvider;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IAlerter _alerter;
        private readonly IFeatureFacade _featureFacade;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IContextFacade contextFacade,
            ICategoryStore<Channel> categoryStore,
            IViewProviderManager<CategoryBase> viewProvider,
            IBreadCrumbManager breadCrumbManager,
            IAlerter alerter, 
            ICategoryManager<Channel> categoryManager, IFeatureFacade featureFacade)
        {
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
            _viewProvider = viewProvider;
            _alerter = alerter;
            _categoryManager = categoryManager;
            _featureFacade = featureFacade;
            _breadCrumbManager = breadCrumbManager;

            T = htmlLocalizer;
            S = stringLocalizer;
        }

        public async Task<IActionResult> Index(int id)
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}
            
            IEnumerable<Channel> parents = null;
            if (id > 0)
            {
                parents = await _categoryStore.GetParentsByIdAsync(id);
            }

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                );

                if (parents == null)
                {
                    builder.Add(S["Channels"]);
                }
                else
                {
                    builder.Add(S["Channels"], channels => channels
                        .Action("Index", "Admin", "Plato.Discuss.Channels", new RouteValueDictionary { ["Id"] = 0 })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        if (parent.Id != id)
                        {
                            builder.Add(S[parent.Name], channel => channel
                                .Action("Index", "Admin", "Plato.Discuss.Channels", new RouteValueDictionary { ["Id"] = parent.Id })
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

            Channel currentChannel = null;
            if (id > 0)
            {
                currentChannel = await _categoryStore.GetByIdAsync(id);
            }
            
            var model = await _viewProvider.ProvideIndexAsync(currentChannel ?? new Channel(), this);
            return View(model);
        }
        
        public async Task<IActionResult> Create(int id = 0)
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
            
            // We need to pass along the featureId
            var feature = await GetcurrentFeature();
            var model = await _viewProvider.ProvideEditAsync(new CategoryBase
            {
                ParentId = id,
                FeatureId = feature.Id

            }, this);
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

            var iconCss = viewModel.IconCss;
            if (!string.IsNullOrEmpty(iconCss))
            {
                iconCss = viewModel.IconPrefix + iconCss;
            }

            var feature = await GetcurrentFeature();
            var category =  new Channel()
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

            var result = await _categoryManager.DeleteAsync(currentCategory);

            if (result.Succeeded)
            {
                _alerter.Success(T["Channel Deleted Successfully"]);
            }
            else
            {

                _alerter.Danger(T["Could not delete the channel"]);
            }

            return RedirectToAction(nameof(Index));
        }
        
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
                _alerter.Success(T["Channel Updated Successfully"]);
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
                _alerter.Success(T["Channel Updated Successfully"]);
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

        async Task<IShellFeature> GetcurrentFeature()
        {
            var featureId = "Plato.Discuss.Channels";
            var feature = await _featureFacade.GetFeatureByIdAsync(featureId);
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the Id '{featureId}'");
            }
            return feature;
        }
        
    }

}
