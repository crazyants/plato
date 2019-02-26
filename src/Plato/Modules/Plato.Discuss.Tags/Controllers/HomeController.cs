using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Discuss.Tags.Models;
using Plato.Tags.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Tags.Stores;
using Plato.Discuss.Tags.ViewModels;
using Plato.Discuss.ViewModels;
using Plato.Entities.ViewModels;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Tags.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

       
        private readonly IViewProviderManager<DiscussTag> _tagViewProvider;
        private readonly ITagStore<Tag> _tagStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IAlerter _alerter;
        private readonly IContextFacade _contextFacade;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public HomeController(
            IViewProviderManager<DiscussTag> tagViewProvider,
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            ITagStore<Tag> tagStore,
            IContextFacade contextFacade,
            IAlerter alerter,
            IBreadCrumbManager breadCrumbManager, 
            IContextFacade contextFacade1)
        {
            _tagStore = tagStore;
            _tagViewProvider = tagViewProvider;
            _alerter = alerter;
            _breadCrumbManager = breadCrumbManager;
            _contextFacade = contextFacade1;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

   
        #region "Actions"

        public async Task<IActionResult> Index(
            int offset,
            TagIndexOptions opts,
            PagerOptions pager)
        {

            if (opts == null)
            {
                opts = new TagIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }

            if (offset > 0)
            {
                pager.Page = offset.ToSafeCeilingDivision(pager.PageSize);
                pager.SelectedOffset = offset;
            }

            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                        .Action("Index", "Home", "Plato.Core")
                        .LocalNav()
                    ).Add(S["Discuss"], discuss => discuss
                        .Action("Index", "Home", "Plato.Discuss")
                        .LocalNav()
                    ).Add(S["Tags"]);
            });
            
            // Get default options
            var defaultViewOptions = new TagIndexOptions();
            var defaultPagerOptions = new PagerOptions();

            // Add non default route data for pagination purposes
            if (opts.Search != defaultViewOptions.Search)
                this.RouteData.Values.Add("opts.search", opts.Search);
            if (opts.Sort != defaultViewOptions.Sort)
                this.RouteData.Values.Add("opts.sort", opts.Sort);
            if (opts.Order != defaultViewOptions.Order)
                this.RouteData.Values.Add("opts.order", opts.Order);
            if (pager.Page != defaultPagerOptions.Page)
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.PageSize != defaultPagerOptions.PageSize)
                this.RouteData.Values.Add("pager.size", pager.PageSize);

            // Build infinate scroll options
            opts.Scroll = new ScrollOptions
            {
                Url = GetInfiniteScrollCallbackUrl()
            };
            
            // Build view model
            var viewModel = new TagIndexViewModel()
            {
                Options = opts,
                Pager = pager
            };

            // Add view options to context for use within view adaptors
            HttpContext.Items[typeof(TagIndexViewModel)] = viewModel;
            
            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetTags", viewModel);
            }
            
            // Return view
            return View(await _tagViewProvider.ProvideIndexAsync(new DiscussTag(), this));

        }
        
        public async Task<IActionResult> Display(
            int id,
            int offset,
            EntityIndexOptions opts,
            PagerOptions pager)
        {

            var tag = await _tagStore.GetByIdAsync(id);
            if (tag == null)
            {
                return NotFound();
            }


            if (opts == null)
            {
                opts = new EntityIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            if (offset > 0)
            {
                pager.Page = offset.ToSafeCeilingDivision(pager.PageSize);
                pager.SelectedOffset = offset;
            }
            
            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                        .Action("Index", "Home", "Plato.Core")
                        .LocalNav()
                    ).Add(S["Discuss"], discuss => discuss
                        .Action("Index", "Home", "Plato.Discuss")
                        .LocalNav()
                    ).Add(S["Tags"], labels => labels
                        .Action("Index", "Home", "Plato.Discuss.Tags")
                        .LocalNav()
                    ).Add(S[tag.Name]);
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

            // Build infinate scroll options
            pager.Scroll = new ScrollOptions
            {
                Url = GetInfiniteScrollCallbackUrl()
            };
            
            // We don't need to add to pagination 
            opts.TagId = tag?.Id ?? 0;

            // Build view model
            var viewModel = new EntityIndexViewModel<Topic>()
            {
                Options = opts,
                Pager = pager
            };

            // Add view options to context for use within view adaptors
            HttpContext.Items[typeof(EntityIndexViewModel<Topic>)] = viewModel;

            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetTopics", viewModel);
            }
            
            // Return view
            return View(await _tagViewProvider.ProvideDisplayAsync(new DiscussTag(tag), this));

        }
        
        #endregion
        
        #region "Private Methods"

        string GetInfiniteScrollCallbackUrl()
        {
            RouteData.Values.Remove("pager.page");
            RouteData.Values.Remove("offset");
            return _contextFacade.GetRouteUrl(RouteData.Values);
        }

        #endregion

    }

}
