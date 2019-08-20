using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Categories.Stores;
using Plato.Docs.Categories.Models;
using Plato.Docs.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.Titles;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Docs.Categories.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {
     
        private readonly IViewProviderManager<Category> _channelViewProvider;
        private readonly ICategoryStore<Category> _channelStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IPageTitleBuilder _pageTitleBuilder;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
 
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IStringLocalizer stringLocalizer,
            IHtmlLocalizer<HomeController> localizer,
            IViewProviderManager<Category> channelViewProvider,
            IBreadCrumbManager breadCrumbManager,
            ICategoryStore<Category> channelStore,
            IPageTitleBuilder pageTitleBuilder,
            IContextFacade contextFacade1, 
            IFeatureFacade featureFacade)
        {
      
            _channelViewProvider = channelViewProvider;
            _breadCrumbManager = breadCrumbManager;
            _pageTitleBuilder = pageTitleBuilder;
            _contextFacade = contextFacade1;
            _featureFacade = featureFacade;
            _channelStore = channelStore;
       
            T = localizer;
            S = stringLocalizer;
        }

        // -----------------
        // Index 
        // -----------------

        public async Task<IActionResult> Index(EntityIndexOptions opts, PagerOptions pager)
        {

            // Build options
            if (opts == null)
            {
                opts = new EntityIndexOptions();
            }

            // Build pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Get category
            var category = await _channelStore.GetByIdAsync(opts.CategoryId);

            // If supplied ensure category exists
            if (category == null && opts.CategoryId > 0)
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
            var viewModel = await GetIndexViewModelAsync(category, opts, pager);

            // Add view model to context
            HttpContext.Items[typeof(EntityIndexViewModel<Doc>)] = viewModel;

            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0 && !pager.Enabled)
                    return View("GetDocs", viewModel);
            }

            // Return Url for authentication purposes
            ViewData["ReturnUrl"] = _contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Docs.Categories",
                ["controller"] = "Home",
                ["action"] = "Index",
                ["opts.id"] = category != null ? category.Id.ToString() : string.Empty,
                ["opts.alias"] = category != null ? category.Alias.ToString() : string.Empty
            });

            // Build page title
            if (category != null)
            {
                _pageTitleBuilder.AddSegment(S[category.Name], int.MaxValue);
            }
            
            // Build breadcrumb
            _breadCrumbManager.Configure(async builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Docs"], home => home
                    .Action("Index", "Home", "Plato.Docs")
                    .LocalNav()
                );

                // Build breadcrumb
                var parents = category != null
                    ? await _channelStore.GetParentsByIdAsync(category.Id)
                    : null;
                if (parents == null)
                {
                    builder.Add(S["Categories"]);
                }
                else
                {

                    builder.Add(S["Categories"], c => c
                        .Action("Index", "Home", "Plato.Docs.Categories", new RouteValueDictionary()
                        {
                            ["opts.categoryId"] = null,
                            ["opts.alias"] = null
                        })
                        .LocalNav()
                    );
                    
                    foreach (var parent in parents)
                    {
                        if (parent.Id != category.Id)
                        {
                            builder.Add(S[parent.Name], channel => channel
                                .Action("Index", "Home", "Plato.Docs.Categories", new RouteValueDictionary
                                {
                                    ["opts.categoryId"] = parent.Id,
                                    ["opts.alias"] = parent.Alias,
                                })
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
            
            // Return view
            return View((LayoutViewModel) await _channelViewProvider.ProvideIndexAsync(category, this));

        }

        // ---------------

        async Task<EntityIndexViewModel<Doc>> GetIndexViewModelAsync(Category category, EntityIndexOptions options, PagerOptions pager)
        {
            
            // Get current feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs");

            // Restrict results to current feature
            if (feature != null)
            {
                options.FeatureId = feature.Id;
            }
            
            // Include child categories
            if (category != null)
            {
                if (category.Children.Any())
                {
                    // Convert child ids to list and add current id
                    var ids = category
                        .Children
                        .Select(c => c.Id).ToList();
                    ids.Add(category.Id);
                    options.CategoryIds = ids.ToArray();
                }
                else
                {
                    options.CategoryId = category.Id;
                }
            }
            else
            {
                options.CategoryId = 0;
            }

            // Ensure pinned entities appear first
            if (options.Sort == SortBy.LastReply)
            {
                options.SortColumns.Add(SortBy.IsPinned.ToString(), OrderBy.Desc);
            }
            
            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            // Return updated model
            return new EntityIndexViewModel<Doc>()
            {
                Options = options,
                Pager = pager
            };

        }
        
    }

}
