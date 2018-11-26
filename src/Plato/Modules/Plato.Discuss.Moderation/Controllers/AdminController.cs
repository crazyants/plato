using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Plato.Discuss.Moderation.ViewModels;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Moderation.Models;
using Plato.Moderation.Stores;
using Plato.WebApi.Models;

namespace Plato.Discuss.Moderation.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {
        private readonly IViewProviderManager<Moderator> _viewProvider;
        private readonly IPlatoUserStore<User> _userStore;
        private readonly IModeratorStore<Moderator> _moderatorStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
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

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Moderators"]);
            });

            var model = await _viewProvider.ProvideIndexAsync(new Moderator(), this);
            return View(model);

        }

        public async Task<IActionResult> Create()
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Moderators"], moderators => moderators
                    .Action("Index", "Admin", "Plato.Discuss.Moderation")
                    .LocalNav()
                ).Add(S["Add Moderator"]);
                ;
            });

            var model = await _viewProvider.ProvideEditAsync(new Moderator(), this);
            return View(model);

        }

        [HttpPost]
        [ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditModeratorViewModel model)
        {

            // Build users to effect
            var users = new List<User>();
            if (!String.IsNullOrEmpty(model.Users))
            {
                var items = JsonConvert.DeserializeObject<IEnumerable<UserApiResult>>(model.Users);
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


            // Ensure we have users
            if (users.Count > 0)
            {

                var isValid = false;

                // Update each user
                foreach (var user in users)
                {

                    //var newModerator = new Moderator()
                    //{
                    //    UserId = user.Id
                    //};

                    // Compose moderator from all involved view providers
                    var composedModerator = await _viewProvider.GetComposedType(this);
                    composedModerator.UserId = user.Id;

                    // Validate model state within all view providers
                    if (await _viewProvider.IsModelStateValid(composedModerator, this))
                    {
                        // Create moderator
                        var moderator = await _moderatorStore.CreateAsync(composedModerator);
                        if (moderator != null)
                        {
                            // Update moderator within various view providers
                            await _viewProvider.ProvideUpdateAsync(moderator, this);
                            isValid = true;
                        }
                    }
                }

                if (isValid)
                {

                    // Everything was OK
                    _alerter.Success(T["Moderator Created Successfully!"]);

                    // Redirect to topic
                    return RedirectToAction(nameof(Index), new {Id = 0});

                }

                // if we reach this point some view model validation
                // failed within a view provider, display model state errors
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _alerter.Danger(T[error.ErrorMessage]);
                    }
                }

                return await Create();


            }
            else
            {
                _alerter.Danger(T["You must specify at least 1 user!"]);
            }

            return await Create();

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
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Moderators"], moderators => moderators
                    .Action("Index", "Admin", "Plato.Discuss.Moderation")
                    .LocalNav()
                ).Add(S["Edit Moderator"]);
            });
            
            var result = await _viewProvider.ProvideEditAsync(moderator, this);
            return View(result);
        }
        
        [HttpPost]
        [ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(int id)
        {

            var moderator = await _moderatorStore.GetByIdAsync(id);
            if (moderator == null)
            {
                return NotFound();
            }

            var result = await _viewProvider.ProvideUpdateAsync(moderator, this);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            _alerter.Success(T["Moderator Updated Successfully!"]);

            return RedirectToAction(nameof(Index));


        }

        [HttpPost]
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


    }

}
