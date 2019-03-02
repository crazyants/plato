using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Badges;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Users.Badges.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IContextFacade _contextFacade;
        private readonly IViewProviderManager<Badge> _badgeViewProvider;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IAlerter _alerter;
        private readonly IPlatoUserStore<User> _platoUserStore;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public HomeController(
            IViewProviderManager<Badge> badgeViewProvider,
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
            _badgeViewProvider = badgeViewProvider;
            _alerter = alerter;
            _breadCrumbManager = breadCrumbManager;
            _platoUserStore = platoUserStore;
            _contextFacade = contextFacade1;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        public async Task<IActionResult> Index()
        {
         
            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Badges"]);
            });

            // Build view
            var result = await _badgeViewProvider.ProvideIndexAsync(new Badge(), this);

            // Return view
            return View(result);

        }
        
        #endregion

    }

}
