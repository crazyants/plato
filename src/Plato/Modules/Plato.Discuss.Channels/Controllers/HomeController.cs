using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Discuss.ViewModels;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Discuss.Channels.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {
     
        private readonly IViewProviderManager<Channel> _channelViewProvider;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly ICategoryStore<Channel> _channelStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IAlerter _alerter;
        private readonly IContextFacade _contextFacade;
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IViewProviderManager<Channel> channelViewProvider,
            IStringLocalizer<AdminController> stringLocalizer,
            IHtmlLocalizer<HomeController> localizer,
            IBreadCrumbManager breadCrumbManager,
            ICategoryStore<Channel> channelStore,
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            IAlerter alerter,
            IContextFacade contextFacade1)
        {
            _settingsStore = settingsStore;
            _channelStore = channelStore;
            _channelViewProvider = channelViewProvider;
            _breadCrumbManager = breadCrumbManager;
            _alerter = alerter;
            _contextFacade = contextFacade1;

            T = localizer;
            S = stringLocalizer;
        }
        
        public async Task<IActionResult> Index(
            int id,
            int offset,
            TopicIndexOptions opts,
            PagerOptions pager)
        {

            var category = new Channel();
            if (id > 0)
            {
                category = await _channelStore.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound();
                }
            }

            if (opts == null)
            {
                opts = new TopicIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }


            // Check permissions

            // Build breadcrumb
            IEnumerable<Channel> parents = null;
            if (id > 0)
            {
                parents = await _channelStore.GetParentsByIdAsync(id);
            }
            _breadCrumbManager.Configure(builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Discuss"], home => home
                    .Action("Index", "Home", "Plato.Discuss")
                    .LocalNav()
                );

                if (parents == null)
                {
                    builder.Add(S["Channels"]);
                }
                else
                {

                    builder.Add(S["Channels"], channels => channels
                        .Action("Index", "Home", "Plato.Discuss.Channels", new RouteValueDictionary()
                        {
                            ["id"] = "",
                            ["alias"] = ""
                        })
                        .LocalNav()
                    );

                    foreach (var parent in parents)
                    {
                        if (parent.Id != id)
                        {
                            builder.Add(S[parent.Name], channel => channel
                                .Action("Index", "Home", "Plato.Discuss.Channels", new RouteValueDictionary
                                {
                                    ["id"] = parent.Id,
                                    ["alias"] = parent.Alias,
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

            // Get default options
            var defaultViewOptions = new TopicIndexOptions();
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
            opts.Scroller = new ScrollerOptions
            {
                Url = GetInfiniteScrollCallbackUrl()
            };

            // Include child channels
            if (category.Children.Any())
            {
                // Convert child ids to list and add current id
                var ids = category
                    .Children
                    .Select(c => c.Id).ToList();
                ids.Add(category.Id);
                opts.Params.ChannelIds = ids.ToArray();
            }
            else
            {
                opts.Params.ChannelId = category.Id;
            }
    
            // Build view model
            var viewModel = new TopicIndexViewModel()
            {
                Options = opts,
                Pager = pager
            };

            // Add view options to context for use within view adaptors
            HttpContext.Items[typeof(TopicIndexViewModel)] = viewModel;
            
            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetTopics", viewModel);
            }
            
            // Return view
            return View(await _channelViewProvider.ProvideIndexAsync(category, this));
            
        }

        string GetInfiniteScrollCallbackUrl()
        {

            RouteData.Values.Remove("pager.page");
            RouteData.Values.Remove("offset");

            return _contextFacade.GetRouteUrl(RouteData.Values);

        }


    }

}
