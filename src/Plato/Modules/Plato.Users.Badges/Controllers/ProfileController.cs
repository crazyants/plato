using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.Titles;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Badges;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.Badges.Controllers
{

    public class ProfileController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IContextFacade _contextFacade;
        private readonly IViewProviderManager<UserBadge> _userBadgeViewProvider;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IPageTitleBuilder _pageTitleBuilder;
        private readonly IPlatoUserStore<User> _platoUserStore;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public ProfileController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IViewProviderManager<UserBadge> userBadgeViewProvider,
            IBreadCrumbManager breadCrumbManager,
            IPlatoUserStore<User> platoUserStore,
            IPageTitleBuilder pageTitleBuilder,
            IContextFacade contextFacade)
        {
            _userBadgeViewProvider = userBadgeViewProvider;
            _breadCrumbManager = breadCrumbManager;
            _pageTitleBuilder = pageTitleBuilder;
            _platoUserStore = platoUserStore;
            _contextFacade = contextFacade;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        public async Task<IActionResult> Index(DisplayUserOptions opts)
        {

            if (opts == null)
            {
                opts = new DisplayUserOptions();
            }

            var user = opts.Id > 0
                ? await _platoUserStore.GetByIdAsync(opts.Id)
                : await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            // Build page title
            _pageTitleBuilder.AddSegment(S[user.DisplayName], int.MaxValue);

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
                        ["opts.id"] = user.Id,
                        ["opts.alias"] = user.Alias
                    })
                    .LocalNav()
                ).Add(S["Badges"]);
            });

            // Return view
            return View((LayoutViewModel) await _userBadgeViewProvider.ProvideIndexAsync(new UserBadge()
            {
                UserId = user.Id
            }, this));

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
                        ["opts.id"] = user.Id,
                        ["opts.alias"] = user.Alias
                    })
                    .LocalNav()
                ).Add(S["Badges"]);
            });

            // Return view
            return View((LayoutViewModel) await _userBadgeViewProvider.ProvideDisplayAsync(new UserBadge()
            {
                UserId = user.Id
            }, this));

        }

        #endregion

    }
    
}
