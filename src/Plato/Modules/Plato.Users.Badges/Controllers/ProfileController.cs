using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Badges;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Users.Badges.Controllers
{
    public class ProfileController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IContextFacade _contextFacade;
        private readonly IViewProviderManager<UserBadge> _userBadgeViewProvider;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IAlerter _alerter;
        private readonly IPlatoUserStore<User> _platoUserStore;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public ProfileController(
            IViewProviderManager<UserBadge> userBadgeViewProvider,
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            IAlerter alerter,
            IBreadCrumbManager breadCrumbManager,
            IPlatoUserStore<User> platoUserStore,
            IContextFacade contextFacade1)
        {
            _settingsStore = settingsStore;
            _userBadgeViewProvider = userBadgeViewProvider;
            _alerter = alerter;
            _breadCrumbManager = breadCrumbManager;
            _platoUserStore = platoUserStore;
            _contextFacade = contextFacade1;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        public async Task<IActionResult> Index(int id)
        {

            var user = id > 0
                ? await _platoUserStore.GetByIdAsync(id)
                : await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Users"], discuss => discuss
                    .Action("Index", "Home", "Plato.Users")
                    .LocalNav()
                ).Add(S[user.DisplayName], discuss => discuss
                    .Action("Display", "Home", "Plato.Users", new RouteValueDictionary()
                    {
                        ["id"] = user.Id,
                        ["alias"] = user.Alias
                    })
                    .LocalNav()
                ).Add(S["Badges"]);
            });

            // Build view
            var result = await _userBadgeViewProvider.ProvideIndexAsync(new UserBadge()
            {
                UserId = user.Id
            }, this);

            // Return view
            return View(result);

        }

        public async Task<IActionResult> Display(int id)
        {

            var user = id > 0
                ? await _platoUserStore.GetByIdAsync(id)
                : await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Users"], discuss => discuss
                    .Action("Index", "Home", "Plato.Users")
                    .LocalNav()
                ).Add(S[user.DisplayName], discuss => discuss
                    .Action("Display", "Home", "Plato.Users", new RouteValueDictionary()
                    {
                        ["id"] = user.Id,
                        ["alias"] = user.Alias
                    })
                    .LocalNav()
                ).Add(S["Badges"]);
            });

            // Build view
            var result = await _userBadgeViewProvider.ProvideDisplayAsync(new UserBadge()
            {
                UserId = user.Id
            }, this);

            // Return view
            return View(result);

        }


        //public async Task<IActionResult> Display(
        //    int id,
        //    TopicIndexOptions opts,
        //    PagerOptions pager)
        //{

        //    var label = await _labelStore.GetByIdAsync(id);
        //    if (label == null)
        //    {
        //        return NotFound();
        //    }

        //    // Breadcrumb
        //    _breadCrumbManager.Configure(builder =>
        //    {
        //        builder.Add(S["Home"], home => home
        //                .Action("Index", "Home", "Plato.Core")
        //                .LocalNav()
        //            ).Add(S["Discuss"], discuss => discuss
        //                .Action("Index", "Home", "Plato.Discuss")
        //                .LocalNav()
        //            ).Add(S["Labels"], labels => labels
        //                .Action("Index", "Home", "Plato.Discuss.Labels")
        //                .LocalNav()
        //            ).Add(S[label.Name]);
        //    });

        //    // Get default options
        //    var defaultViewOptions = new TopicIndexOptions();
        //    var defaultPagerOptions = new PagerOptions();

        //    // Add non default route data for pagination purposes
        //    if (opts.Search != defaultViewOptions.Search)
        //        this.RouteData.Values.Add("opts.search", opts.Search);
        //    if (opts.Sort != defaultViewOptions.Sort)
        //        this.RouteData.Values.Add("opts.sort", opts.Sort);
        //    if (opts.Order != defaultViewOptions.Order)
        //        this.RouteData.Values.Add("opts.order", opts.Order);
        //    if (opts.Filter != defaultViewOptions.Filter)
        //        this.RouteData.Values.Add("opts.filter", opts.Filter);
        //    if (pager.Page != defaultPagerOptions.Page)
        //        this.RouteData.Values.Add("pager.page", pager.Page);
        //    if (pager.PageSize != defaultPagerOptions.PageSize)
        //        this.RouteData.Values.Add("pager.size", pager.PageSize);

        //    // We don't need to add to pagination 
        //    opts.Params.LabelId = label?.Id ?? 0;

        //    // Add view options to context for use within view adaptors
        //    this.HttpContext.Items[typeof(TopicIndexViewModel)] = new TopicIndexViewModel()
        //    {
        //        Options = opts,
        //        Pager = pager
        //    };

        //    // Build view
        //    var result = await _labelViewProvider.ProvideDisplayAsync(label, this);

        //    // Return view
        //    return View(result);

        //}


        #endregion

    }


}
