using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Labels.Models;
using Plato.Discuss.Labels.ViewModels;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Services;
using Plato.Labels.Stores;

namespace Plato.Discuss.Labels.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {
        private readonly IContextFacade _contextFacade;
        private readonly ILabelStore<LabelBase> _labelStore;
        private readonly ILabelManager<LabelBase> _labelManager;
        private readonly IViewProviderManager<LabelBase> _viewProvider;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IFeatureFacade _featureFacade;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IContextFacade contextFacade,
            ILabelStore<LabelBase> labelStore,
            IViewProviderManager<LabelBase> viewProvider,
            IBreadCrumbManager breadCrumbManager,
            IAlerter alerter,
            ILabelManager<LabelBase> labelManager,
            IFeatureFacade featureFacade)
        {
            _contextFacade = contextFacade;
            _labelStore = labelStore;
            _viewProvider = viewProvider;
            _alerter = alerter;
            _labelManager = labelManager;
            _featureFacade = featureFacade;
            _breadCrumbManager = breadCrumbManager;

            T = htmlLocalizer;
            S = stringLocalizer;
        }

        public async Task<IActionResult> Index(
            int offset,
            LabelIndexOptions opts,
            PagerOptions pager)
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}
            
            if (opts == null)
            {
                opts = new LabelIndexOptions();
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
                    .Action("Index", "Admin", "Plato.Admin")
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


            // Build infinate scroll options
            opts.InfiniteScroll = new InfiniteScrollOptions
            {
                Url = GetInfiniteScrollCallbackUrl()
            };

            // Indicate administrator view
            opts.EnableEdit = true;
            
            // Build view model
            var viewModel = new LabelIndexViewModel()
            {
                Options = opts,
                Pager = pager
            };

            // Add view options to context for use within view adaptors
            HttpContext.Items[typeof(LabelIndexViewModel)] = viewModel;
            
            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetLabels", viewModel);
            }

            // Return view
            return View(await _viewProvider.ProvideIndexAsync(new LabelBase(), this));

        }
        
        public async Task<IActionResult> Create()
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Labels"], labels => labels
                    .Action("Index", "Admin", "Plato.Discuss.Labels")
                    .LocalNav()
                ).Add(S["Add Label"]);
            });
            
            // We need to pass along the featureId
            var model = await _viewProvider.ProvideEditAsync(new Label
            {
                FeatureId = await GetFeatureIdAsync() 

            }, this);
            return View(model);

        }

        [HttpPost]
        [ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditLabelViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            
            var category =  new Label()
            {
                FeatureId = await GetFeatureIdAsync(),
                Name = viewModel.Name,
                Description = viewModel.Description,
                ForeColor = viewModel.ForeColor,
                BackColor = viewModel.BackColor
            };

            var result = await _labelManager.CreateAsync(category);
            if (result.Succeeded)
            {

                await _viewProvider.ProvideUpdateAsync(result.Response, this);

                _alerter.Success(T["Label Added Successfully!"]);

                return RedirectToAction(nameof(Index));

            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(viewModel);
            
        }
        
        public async Task<IActionResult> Edit(int id)
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Labels"], labels => labels
                    .Action("Index", "Admin", "Plato.Discuss.Labels")
                    .LocalNav()
                ).Add(S["Edit Label"]);
            });
            
            var category = await _labelStore.GetByIdAsync(id);
            var model = await _viewProvider.ProvideEditAsync(category, this);
            return View(model);

        }

        [HttpPost]
        [ActionName(nameof(Edit))]
        public  async Task<IActionResult> EditPost(int id)
        {

            var currentCategory = await _labelStore.GetByIdAsync(id);
            if (currentCategory == null)
            {
                return NotFound();
            }

            var result = await _viewProvider.ProvideUpdateAsync(currentCategory, this);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            _alerter.Success(T["Label Updated Successfully!"]);

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            
            var ok = int.TryParse(id, out int categoryId);
            if (!ok)
            {
                return NotFound();
            }

            var currentLabel = await _labelStore.GetByIdAsync(categoryId);

            if (currentLabel == null)
            {
                return NotFound();
            }

            var result = await _labelManager.DeleteAsync(currentLabel);

            if (result.Succeeded)
            {
                _alerter.Success(T["Label Deleted Successfully"]);
            }
            else
            {

                _alerter.Danger(T["Could not delete the label"]);
            }

            return RedirectToAction(nameof(Index));
        }
        
        async Task<int> GetFeatureIdAsync()
        {
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Labels");
            if (feature != null)
            {
                return feature.Id;
            }

            throw new Exception($"Could not find required feture registration for Plato.Discuss.Labels");
        }
        
        string GetInfiniteScrollCallbackUrl()
        {

            RouteData.Values.Remove("pager.page");
            RouteData.Values.Remove("offset");

            return _contextFacade.GetRouteUrl(RouteData.Values);

        }

    }

}
