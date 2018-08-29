using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Plato.Discuss.Moderation.Models;
using Plato.Discuss.Moderation.ViewModels;
using Plato.Discuss.Moderation.ViewProviders;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Moderation.Models;

namespace Plato.Discuss.Moderation.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {
        private readonly IViewProviderManager<Moderator> _viewProvider;
        private readonly IPlatoUserStore<User> _userStore;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }


        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IViewProviderManager<Moderator> viewProvider,
            IAlerter alerter,
            IPlatoUserStore<User> userStore)
        {

            _viewProvider = viewProvider;
            _alerter = alerter;
            _userStore = userStore;

            T = htmlLocalizer;
            S = stringLocalizer;

        }
        
        public async Task<IActionResult> Index()
        {
            
            var model = await _viewProvider.ProvideIndexAsync(new Moderator(), this);
            return View(model);

        }
        
        public async Task<IActionResult> Create()
        {
         
            var model = await _viewProvider.ProvideEditAsync(new Moderator(), this);
            return View(model);

        }

        [HttpPost]
        [ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditModeratorViewModel model)
        {
        
            // Build users to effect
            var users = new List<User>();
            var items = JsonConvert.DeserializeObject<IEnumerable<TagItItem>>(model.Users);
            foreach (var item in items)
            {
                if (!String.IsNullOrEmpty(item.Value))
                {
                    var user = await _userStore.GetByUserNameAsync(item.Value);
                    if (user != null)
                    {
                        users.Add(user);
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

                    // Validate model state within all view providers
                    if (await _viewProvider.IsModelStateValid(new Moderator()
                    {
                        UserId = user.Id
                    }, this))
                    {

                        // Get fully composed type from all involved view providers
                        var moderator = await _viewProvider.GetComposedType(this);

                        // Update moderator
                        await _viewProvider.ProvideUpdateAsync(moderator, this);

                        // Was there a problem updating the entity?
                        if (moderator != null)
                        {
                            isValid = true;
                        }

                    }
                }

                if (isValid)
                {

                    // Everything was OK
                    _alerter.Success(T["Moderator Created Successfully!"]);

                    // Redirect to topic
                    return RedirectToAction(nameof(Index), new { Id = 0 });

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

            return RedirectToAction(nameof(Index));

        }

    }

}
