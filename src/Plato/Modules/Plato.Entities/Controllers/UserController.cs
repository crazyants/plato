using System.Threading.Tasks;
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
using Plato.Entities.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Titles;

namespace Plato.Entities.Controllers
{
    public class UserController : Controller, IUpdateModel
    {
        
        private readonly IViewProviderManager<EntityUserIndex> _userViewProvider;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IContextFacade _contextFacade;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IPageTitleBuilder _pageTitleBuilder;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public UserController(
            IStringLocalizer stringLocalizer,
            IHtmlLocalizer localizer,
            IContextFacade contextFacade,
            IAlerter alerter, IBreadCrumbManager breadCrumbManager,
            IPlatoUserStore<User> platoUserStore,
            IViewProviderManager<EntityUserIndex> userViewProvider,
            IPageTitleBuilder pageTitleBuilder)
        {
            _contextFacade = contextFacade;
            _breadCrumbManager = breadCrumbManager;
            _platoUserStore = platoUserStore;
            _userViewProvider = userViewProvider;
            _pageTitleBuilder = pageTitleBuilder;

            T = localizer;
            S = stringLocalizer;

        }

        public async Task<IActionResult> Index(EntityIndexOptions opts, PagerOptions pager)
        {

            // Default options
            if (opts == null)
            {
                opts = new EntityIndexOptions();
            }

            // Default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Get user
            var user = await _platoUserStore.GetByIdAsync(opts.CreatedByUserId);

            // Ensure user exists
            if (user == null)
            {
                return NotFound();
            }
            
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
            if (pager.Size != defaultPagerOptions.Size)
                this.RouteData.Values.Add("pager.size", pager.Size);
            
            // Build view model
            var viewModel = GetIndexViewModel(opts, pager);

            // Add view model to context
            this.HttpContext.Items[typeof(EntityIndexViewModel<Entity>)] = viewModel;

            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetEntities", viewModel);
            }
            
            // Build page title
            _pageTitleBuilder
                .AddSegment(S["Users"])
                .AddSegment(S[user.DisplayName])
                .AddSegment(S["All"]);

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
                        ["opts.id"] = user.Id,
                        ["opts.alias"] = user.Alias
                    })
                    .LocalNav()
                ).Add(S["All"]);
            });
            

            //// Return view
            return View((LayoutViewModel) await _userViewProvider.ProvideDisplayAsync(new EntityUserIndex()
            {
                Id = user.Id
            }, this));

        }

        EntityIndexViewModel<Entity> GetIndexViewModel(EntityIndexOptions options, PagerOptions pager)
        {
            
            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            // Ensure we have a default sort column
            if (options.Sort == SortBy.Auto)
            {
                options.Sort = SortBy.LastReply;
            }

            // Return updated model
            return new EntityIndexViewModel<Entity>()
            {
                Options = options,
                Pager = pager
            };

        }
        
    }

}
