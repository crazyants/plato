using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Labels.Services;
using Plato.Labels.Stores;
using Plato.Labels.ViewModels;
using Plato.Docs.Labels.Models;
using Plato.Docs.Labels.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Layout;

namespace Plato.Docs.Labels.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly IContextFacade _contextFacade;
        private readonly ILabelStore<Label> _labelStore;
        private readonly ILabelManager<Label> _labelManager;
        private readonly IViewProviderManager<LabelAdmin> _viewProvider;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IFeatureFacade _featureFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IContextFacade contextFacade,
            ILabelStore<Label> labelStore,
            ILabelManager<Label> labelManager,
            IViewProviderManager<LabelAdmin> viewProvider,
            IBreadCrumbManager breadCrumbManager,
            IFeatureFacade featureFacade,
            IAlerter alerter)
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

        // ------------
        // Index
        // ------------

        public async Task<IActionResult> Index(LabelIndexOptions opts, PagerOptions pager)
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
            if (pager.Size != defaultPagerOptions.Size)
                this.RouteData.Values.Add("pager.size", pager.Size);
            
            // Build view model
            var viewModel = await GetIndexViewModelAsync(opts, pager);

            // Add view model to context
            HttpContext.Items[typeof(LabelIndexViewModel<Label>)] = viewModel;
            
            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetLabels", viewModel);
            }
            
            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Docs"], docs => docs
                        .Action("Index", "Admin", "Plato.Docs")
                        .LocalNav())
                    .Add(S["Labels"]);
            });
            
            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideIndexAsync(new Label(), this));

        }

        // ------------
        // Create
        // ------------

        public async Task<IActionResult> Create()
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Docs"], docs => docs
                        .Action("Index", "Admin", "Plato.Docs")
                        .LocalNav())
                    .Add(S["Labels"], labels => labels
                        .Action("Index", "Admin", "Plato.Docs.Labels")
                        .LocalNav())
                    .Add(S["Add Label"]);
            });
            
            // We need to pass along the featureId
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(new Label
            {
                FeatureId = await GetFeatureIdAsync()

            }, this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditLabelViewModel viewModel)
        {

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            if (user == null)
            {
                return Unauthorized();
            }


            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            
            var label =  new Label()
            {
                FeatureId = await GetFeatureIdAsync(),
                Name = viewModel.Name,
                Description = viewModel.Description,
                ForeColor = viewModel.ForeColor,
                BackColor = viewModel.BackColor,
                CreatedUserId = user.Id,
                CreatedDate = DateTimeOffset.UtcNow
            };

            var result = await _labelManager.CreateAsync(label);
            if (result.Succeeded)
            {

                // Indicate new label
                result.Response.IsNewLabel = true;

                // Execute view providers
                await _viewProvider.ProvideUpdateAsync(result.Response, this);

                // Add confirmation
                _alerter.Success(T["Label Added Successfully!"]);

                // Return
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

        // ------------
        // Edit
        // ------------

        public async Task<IActionResult> Edit(int id)
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Docs"], docs => docs
                        .Action("Index", "Admin", "Plato.Docs")
                        .LocalNav())
                    .Add(S["Labels"], labels => labels
                        .Action("Index", "Admin", "Plato.Docs.Labels")
                        .LocalNav())
                    .Add(S["Edit Label"]);
            });
            
            var category = await _labelStore.GetByIdAsync(id);
            
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(category, this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
        public  async Task<IActionResult> EditPost(int id)
        {

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            if (user == null)
            {
                return Unauthorized();
            }

            var currentLabel = await _labelStore.GetByIdAsync(id);
            if (currentLabel == null)
            {
                return NotFound();
            }

            currentLabel.ModifiedUserId = user.Id;
            currentLabel.ModifiedDate = DateTimeOffset.UtcNow;

            var result = await _viewProvider.ProvideUpdateAsync(currentLabel, this);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            _alerter.Success(T["Label Updated Successfully!"]);

            return RedirectToAction(nameof(Index));

        }

        // ------------
        // Delete
        // ------------

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            
            var ok = int.TryParse(id, out var categoryId);
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

        // ------------

        async Task<LabelIndexViewModel<Label>> GetIndexViewModelAsync(LabelIndexOptions options, PagerOptions pager)
        {

            // Get feature
            options.FeatureId = await GetFeatureIdAsync();
       
            if (options.Sort == LabelSortBy.Auto)
            {
                options.Sort = LabelSortBy.Modified;
                options.Order = OrderBy.Desc;
            }

            // Indicate administrator view
            options.EnableEdit = true;

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            return new LabelIndexViewModel<Label>()
            {
                Options = options,
                Pager = pager
            };

        }

        async Task<int> GetFeatureIdAsync()
        {
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs");
            if (feature != null)
            {
                return feature.Id;
            }

            throw new Exception($"Could not find required feature registration for Plato.Docs");
        }
        
    }

}
