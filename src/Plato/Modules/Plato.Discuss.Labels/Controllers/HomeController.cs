using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Labels.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Labels.Stores;
using Plato.Discuss.Labels.ViewModels;
using Plato.Discuss.ViewModels;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Discuss.Labels.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"
        
        private readonly IViewProviderManager<Label> _labelViewProvider;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly ILabelStore<Label> _labelStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IAlerter _alerter;


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
            IBreadCrumbManager breadCrumbManager)
        {
            _settingsStore = settingsStore;
            _labelStore = labelStore;
            _labelViewProvider = labelViewProvider;
            _alerter = alerter;
            _breadCrumbManager = breadCrumbManager;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        public async Task<IActionResult> Index(
            LabelIndexOptions opts,
            PagerOptions pager)
        {

            if (opts == null)
            {
                opts = new LabelIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
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

            // Add view options to context for use within view adaptors
            this.HttpContext.Items[typeof(LabelIndexViewModel)] = new LabelIndexViewModel()
            {
                Options = opts,
                Pager = pager
            };

            // Build view
            var result = await _labelViewProvider.ProvideIndexAsync(new Label(), this);

            // Return view
            return View(result);

        }


        public async Task<IActionResult> Display(
            int id,
            TopicIndexOptions opts,
            PagerOptions pager)
        {

            var label = await _labelStore.GetByIdAsync(id);
            if (label == null)
            {
                return NotFound();
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
                    ).Add(S["Labels"], labels => labels
                        .Action("Index", "Home", "Plato.Discuss.Labels")
                        .LocalNav()
                    ).Add(S[label.Name]);
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

            // We don't need to add to pagination 
            opts.Params.LabelId = label?.Id ?? 0;

            // Add view options to context for use within view adaptors
            this.HttpContext.Items[typeof(TopicIndexViewModel)] = new TopicIndexViewModel()
            {
                Options = opts,
                Pager = pager
            };

            // Build view
            var result = await _labelViewProvider.ProvideDisplayAsync(label, this);

            // Return view
            return View(result);

        }


        #endregion

    }

}
