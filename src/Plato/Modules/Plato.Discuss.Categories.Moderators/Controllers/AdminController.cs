using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Plato.Discuss.Categories.Moderators.ViewModels;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Moderation.Models;
using Plato.Moderation.Stores;
using Plato.WebApi.Models;

namespace Plato.Discuss.Categories.Moderators.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {
        private readonly IViewProviderManager<Moderator> _viewProvider;
        private readonly IModeratorStore<Moderator> _moderatorStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IPlatoUserStore<User> _userStore;
        private readonly IAlerter _alerter;
        
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IViewProviderManager<Moderator> viewProvider,
            IModeratorStore<Moderator> moderatorStore,
            IPlatoUserStore<User> userStore,
            IBreadCrumbManager breadCrumbManager,
            IAlerter alerter)
        {

            _viewProvider = viewProvider;
            _userStore = userStore;
            _moderatorStore = moderatorStore;
            _breadCrumbManager = breadCrumbManager;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<IActionResult> Index()
        {

            //var moderators = await _moderatorStore.GetCategorizedModeratorsAsync();
            
            //var claims = "";
            //if (moderators != null)
            //{
            //    foreach (var moderator in moderators)
            //    {
            //        claims += moderator.Key.DisplayName + "<br>";
            //        foreach (var channel in moderator.Value)
            //        {
            //            claims += channel.CategoryId + "<br>";
            //            foreach (var claim in channel.Claims)
            //            {
            //                claims += "- " + claim.ClaimType + " - " + claim.ClaimValue + "<br>";
            //            }
            //        }
            //    }

            //}
            
            //ViewData["claims"] = claims;
            
            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Discuss"], discuss => discuss
                        .Action("Index", "Admin", "Plato.Discuss")
                        .LocalNav())
                    .Add(S["Moderators"]);
            });
            
            return View((LayoutViewModel) await _viewProvider.ProvideIndexAsync(new Moderator(), this));

        }

        public async Task<IActionResult> Create()
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Discuss"], discuss => discuss
                        .Action("Index", "Admin", "Plato.Discuss")
                        .LocalNav())
                    .Add(S["Moderators"], moderators => moderators
                        .Action("Index", "Admin", "Plato.Discuss.Categories.Moderators")
                        .LocalNav())
                    .Add(S["Add Moderator"]);
            });
            
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(new Moderator(), this));

        }

        [HttpPost, ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditModeratorViewModel viewModel)
        {

            // Build users to effect
            var users = new List<User>();
            if (!String.IsNullOrEmpty(viewModel.Users))
            {
                var items = JsonConvert.DeserializeObject<IEnumerable<UserApiResult>>(viewModel.Users);
                foreach (var item in items)
                {
                    if (item.Id > 0)
                    {
                        var user = await _userStore.GetByIdAsync(item.Id);
                        if (user != null)
                        {
                            users.Add(user);
                        }
                    }
                }
            }

            var userId = 0;
            foreach (var user in users)
            {
                userId = user.Id;
            }

            var moderator = new Moderator()
            {
                UserId = userId
            };

            // Validate model state within all involved view providers
            if (await _viewProvider.IsModelStateValidAsync(moderator, this))
            {

                // Get composed type from all involved view providers
                var model = await _viewProvider.ComposeModelAsync(moderator, this);

                // Create moderator
                var result = await _moderatorStore.CreateAsync(model);
                if (result != null)
                {

                    // Update moderator within various view providers
                    await _viewProvider.ProvideUpdateAsync(result, this);

                    // Everything was OK
                    _alerter.Success(T["Moderator Created Successfully!"]);

                    // Redirect to topic
                    return RedirectToAction(nameof(Index), new { Id = 0 });

                }

            }
            else
            {
                _alerter.Danger(T["You must specify at least 1 user!"]);
            }

            return await Create();


            //if (users.Count > 0)
            //{
                
            //    // Compose moderator from all involved view providers
            //    // This ensures the claims are always populated
            //    var composedModerator = await _viewProvider.GetComposedType(this);
            //    var isValid = false;

            //    //// Update each user
            //    //foreach (var user in users)
            //    //{
                    
            //    //    composedModerator.UserId = user.Id;

            //    //    // Validate model state within all view providers
            //    //    if (await _viewProvider.IsModelStateValid(composedModerator, this))
            //    //    {
            //    //        // Create moderator
            //    //        var result = await _moderatorStore.CreateAsync(composedModerator);
            //    //        if (result != null)
            //    //        {
            //    //            // Update moderator within various view providers
            //    //            await _viewProvider.ProvideUpdateAsync(result, this);
            //    //            isValid = true;
            //    //        }
            //    //    }
            //    //}

            //    //if (isValid)
            //    //{

            //    //    // Everything was OK
            //    //    _alerter.Success(T["Moderator Created Successfully!"]);

            //    //    // Redirect to topic
            //    //    return RedirectToAction(nameof(Index), new {Id = 0});

            //    //}

            //    // if we reach this point some view model validation
            //    // failed within a view provider, display model state errors
            //    foreach (var modelState in ViewData.ModelState.Values)
            //    {
            //        foreach (var error in modelState.Errors)
            //        {
            //            _alerter.Danger(T[error.ErrorMessage]);
            //        }
            //    }

            //    return await Create();
                
            //}
            //else
            //{
            //    _alerter.Danger(T["You must specify at least 1 user!"]);
            //}

            //return await Create();

        }
        
        public async Task<IActionResult> Edit(int id)
        {

            var moderator = await _moderatorStore.GetByIdAsync(id);
            if (moderator == null)
            {
                return NotFound();
            }

            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Discuss"], discuss => discuss
                        .Action("Index", "Admin", "Plato.Discuss")
                        .LocalNav())
                    .Add(S["Moderators"], moderators => moderators
                        .Action("Index", "Admin", "Plato.Discuss.Categories.Moderators")
                        .LocalNav())
                    .Add(S["Edit Moderator"]);
            });
            
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(moderator, this));
        }
        
        [HttpPost, ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(int id)
        {

            var moderator = await _moderatorStore.GetByIdAsync(id);
            if (moderator == null)
            {
                return NotFound();
            }
            
            var result = await _viewProvider.ProvideUpdateAsync(moderator, this);
            if (ModelState.IsValid)
            {
                _alerter.Success(T["Moderator Updated Successfully!"]);
                return RedirectToAction(nameof(Index));
            }

            // if we reach this point some view model validation
            // failed within a view provider, display model state errors
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _alerter.Danger(T[error.ErrorMessage]);
                }
            }

            return await Edit(moderator.Id);
            
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {

            var ok = int.TryParse(id, out int moderatorId);
            if (!ok)
            {
                return NotFound();
            }

            var moderator = await _moderatorStore.GetByIdAsync(moderatorId);
            if (moderator == null)
            {
                return NotFound();
            }

            var result = await _moderatorStore.DeleteAsync(moderator);
            if (result)
            {
                _alerter.Success(T["Moderator Deleted Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not delete the moderator"]);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {

            var ok = int.TryParse(id, out int userId);
            if (!ok)
            {
                return NotFound();
            }

            var user = await _userStore.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Get all moderator entries for the given user
            var moderators = await _moderatorStore.QueryAsync()
                .Select<ModeratorQueryParams>(q => { q.UserId.Equals(user.Id); })
                .ToList();

            var result = false;
            if (moderators?.Data != null)
            {
                result = true;
                foreach (var moderator in moderators.Data)
                {
                    if (!await _moderatorStore.DeleteAsync(moderator))
                    {
                        result = false;
                    }
                }
            }

            if (result)
            {
                _alerter.Success(T["Moderator Deleted Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not delete the moderator"]);
            }

            return RedirectToAction(nameof(Index));
        }
        
    }

}
