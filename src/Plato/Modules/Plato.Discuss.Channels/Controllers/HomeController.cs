using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Entities.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Channels.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {
     
        private readonly IViewProviderManager<ChannelHome> _channelViewProvider;
        private readonly ICategoryStore<ChannelHome> _channelStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IViewProviderManager<ChannelHome> channelViewProvider,
            IStringLocalizer stringLocalizer,
            IHtmlLocalizer<HomeController> localizer,
            IBreadCrumbManager breadCrumbManager,
            ICategoryStore<ChannelHome> channelStore,
            ISiteSettingsStore settingsStore,
            IAlerter alerter,
            IContextFacade contextFacade1, 
            IFeatureFacade featureFacade)
        {
            _channelStore = channelStore;
            _channelViewProvider = channelViewProvider;
            _breadCrumbManager = breadCrumbManager;
            _alerter = alerter;
            _contextFacade = contextFacade1;
            _featureFacade = featureFacade;

            T = localizer;
            S = stringLocalizer;
        }

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
            if (pager.PageSize != defaultPagerOptions.PageSize)
                this.RouteData.Values.Add("pager.size", pager.PageSize);

            // Build view model
            var viewModel = await GetIndexViewModelAsync(category, opts, pager);

            // Add view model to context
            HttpContext.Items[typeof(EntityIndexViewModel<Topic>)] = viewModel;

            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0 && !pager.Enabled)
                    return View("GetTopics", viewModel);
            }

            // Build breadcrumb
            _breadCrumbManager.Configure(async builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Discuss"], home => home
                    .Action("Index", "Home", "Plato.Discuss")
                    .LocalNav()
                );

                // Build breadcrumb
                var parents = category != null
                    ? await _channelStore.GetParentsByIdAsync(category.Id)
                    : null;
                if (parents == null)
                {
                    builder.Add(S["Channels"]);
                }
                else
                {

                    builder.Add(S["Channels"], channels => channels
                        .Action("Index", "Home", "Plato.Discuss.Channels", new RouteValueDictionary()
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
                                .Action("Index", "Home", "Plato.Discuss.Channels", new RouteValueDictionary
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
            return View(await _channelViewProvider.ProvideIndexAsync(category, this));

        }

        async Task<EntityIndexViewModel<Topic>> GetIndexViewModelAsync(Channel category, EntityIndexOptions options, PagerOptions pager)
        {
            
            // Get current feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");

            // Restrict results to current feature
            if (feature != null)
            {
                options.FeatureId = feature.Id;
            }
            
            // Include child channels
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

            // Ensure results are sorted
            if (options.Sort == SortBy.Auto)
            {
                options.Sort = SortBy.LastReply;
            }

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            // Return updated model
            return new EntityIndexViewModel<Topic>()
            {
                Options = options,
                Pager = pager
            };

        }
        
    }

}
