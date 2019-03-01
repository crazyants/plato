using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Labels.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Labels.Stores;
using Plato.Discuss.Labels.ViewModels;
using Plato.Discuss.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using SortBy = Plato.Entities.ViewModels.SortBy;

namespace Plato.Discuss.Labels.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<Label> _labelViewProvider;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly ILabelStore<Label> _labelStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IAlerter _alerter;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public HomeController(
            IViewProviderManager<Label> labelViewProvider,
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            ILabelStore<Label> labelStore,
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            IAlerter alerter,
            IBreadCrumbManager breadCrumbManager,
            IContextFacade contextFacade1,
            IFeatureFacade featureFacade)
        {
            _settingsStore = settingsStore;
            _labelStore = labelStore;
            _labelViewProvider = labelViewProvider;
            _alerter = alerter;
            _breadCrumbManager = breadCrumbManager;
            _contextFacade = contextFacade1;
            _featureFacade = featureFacade;

            T = htmlLocalizer;
            S = stringLocalizer;

        }
        
        public async Task<IActionResult> Index(LabelIndexOptions opts, PagerOptions pager)
        {

            if (opts == null)
            {
                opts = new LabelIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                        .Action("Index", "Home", "Plato.Core")
                        .LocalNav()
                    ).Add(S["Discuss"], discuss => discuss
                        .Action("Index", "Home", "Plato.Discuss")
                        .LocalNav()
                    ).Add(S["Labels"]);
            });
            
            // Get default options
            var defaultViewOptions = new LabelIndexOptions();
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
            
            // Build view model
            var viewModel = new LabelIndexViewModel()
            {
                Options = opts,
                Pager = pager
            };

            // Add view options to context 
            HttpContext.Items[typeof(LabelIndexViewModel)] = viewModel;
            
            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetLabels", viewModel);
            }
            
            // Return view
            return View(await _labelViewProvider.ProvideIndexAsync(new Label(), this));

        }

        public async Task<IActionResult> Display(EntityIndexOptions opts, PagerOptions pager)
        {

            // Get label
            var label = await _labelStore.GetByIdAsync(opts.LabelId);

            // Ensure label exists
            if (label == null)
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
            var viewModel = await GetDisplayViewModelAsync(opts, pager);

            // Add view model to context 
            this.HttpContext.Items[typeof(EntityIndexViewModel<Topic>)] = viewModel;

            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0 && !pager.Enabled)
                    return View("GetTopics", viewModel);
            }
            
            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Discuss"], discuss => discuss
                    .Action("Index", "Home", "Plato.Discuss")
                    .LocalNav()
                ).Add(S["Labels"], labels => labels
                    .Action("Index", "Home", "Plato.Discuss.Labels")
                    .LocalNav()
                ).Add(S[label.Name]);
            });


            // Return view
            return View(await _labelViewProvider.ProvideDisplayAsync(label, this));

        }

        async Task<EntityIndexViewModel<Topic>> GetDisplayViewModelAsync(EntityIndexOptions options, PagerOptions pager)
        {

            // Get current feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");

            // Restrict results to current feature
            if (feature != null)
            {
                options.FeatureId = feature.Id;
            }
            
            // Ensure results are sorted
            if (options.Sort  == SortBy.Auto)
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
