using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Articles.Models;
using Plato.Entities.ViewModels;

namespace Plato.Articles.Controllers
{
    public class UserController : Controller, IUpdateModel
    {
        
        private readonly IViewProviderManager<UserIndex> _userViewProvider;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IContextFacade _contextFacade;
        private readonly IAuthorizationService _authorizationService;
        private readonly IPlatoUserStore<User> _platoUserStore;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public UserController(
            IStringLocalizer<HomeController> stringLocalizer,
            IHtmlLocalizer<HomeController> localizer,
            IContextFacade contextFacade,
            IAlerter alerter, IBreadCrumbManager breadCrumbManager,
            IPlatoUserStore<User> platoUserStore,
            IAuthorizationService authorizationService,
            IViewProviderManager<UserIndex> userViewProvider)
        {
            _contextFacade = contextFacade;
            _breadCrumbManager = breadCrumbManager;
            _platoUserStore = platoUserStore;
            _authorizationService = authorizationService;
            _userViewProvider = userViewProvider;

            T = localizer;
            S = stringLocalizer;

        }

        public async Task<IActionResult> Index(int id, EntityIndexOptions opts, PagerOptions pager)
        {

            // Get user
            var user = await _platoUserStore.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // default options
            if (opts == null)
            {
                opts = new EntityIndexOptions();
            }

            // default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Users"], users => users
                    .Action("Index", "Home", "Plato.Users")
                    .LocalNav()
                ).Add(S[user.DisplayName], name => name
                    .Action("Display", "Home", "Plato.Users", new RouteValueDictionary()
                    {
                        ["id"] = user.Id,
                        ["alias"] = user.Alias
                    })
                    .LocalNav()
                ).Add(S["Articles"]);
            });


            // Get default options
            var defaultViewOptions = new EntityIndexOptions();
            var defaultPagerOptions = new PagerOptions();

            // Add non default route data for pagination purposes
            if (opts.Search != defaultViewOptions.Search)
                this.RouteData.Values.Add("opts.search", opts.Search);
            if (opts.Sort != defaultViewOptions.Sort)
                this.RouteData.Values.Add("opts.sort", opts.Sort);
            if (opts.Order != defaultViewOptions.Order)
                this.RouteData.Values.Add("opts.order", opts.Order);
            if (opts.Filter != defaultViewOptions.Filter)
                this.RouteData.Values.Add("opts.filter", opts.Filter);
            if (pager.Page != defaultPagerOptions.Page)
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.PageSize != defaultPagerOptions.PageSize)
                this.RouteData.Values.Add("pager.size", pager.PageSize);

            // Limited results to current user
            opts.CreatedByUserId = user.Id;

            // Build view model
            var viewModel = new EntityIndexViewModel<Article>()
            {
                Options = opts,
                Pager = pager
            };

            // Add view model to context
            this.HttpContext.Items[typeof(EntityIndexViewModel<Article>)] = viewModel;

            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetArticles", viewModel);
            }

            // Build view
            var result = await _userViewProvider.ProvideDisplayAsync(new UserIndex()
            {
                Id = id
            }, this);

            //// Return view
            return View(result);

        }

    }

}
