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
using Plato.Tags.Services;
using Plato.Tags.Stores;
using Plato.Tags.ViewModels;
using Plato.Docs.Tags.Models;
using Plato.Docs.Tags.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Layout;

namespace Plato.Docs.Tags.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly IContextFacade _contextFacade;
        private readonly ITagStore<Tag> _tagStore;
        private readonly ITagManager<Tag> _tagManager;
        private readonly IViewProviderManager<TagAdmin> _viewProvider;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IFeatureFacade _featureFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IContextFacade contextFacade,
            ITagStore<Tag> tagStore,
            ITagManager<Tag> tagManager,
            IViewProviderManager<TagAdmin> viewProvider,
            IBreadCrumbManager breadCrumbManager,
            IFeatureFacade featureFacade,
            IAlerter alerter)
        {
            _contextFacade = contextFacade;
            _tagStore = tagStore;
            _viewProvider = viewProvider;
            _alerter = alerter;
            _tagManager = tagManager;
            _featureFacade = featureFacade;
            _breadCrumbManager = breadCrumbManager;

            T = htmlLocalizer;
            S = stringLocalizer;
        }

        // ------------
        // Index
        // ------------

        public async Task<IActionResult> Index(TagIndexOptions opts, PagerOptions pager)
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}
            
            if (opts == null)
            {
                opts = new TagIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }

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
            if (pager.Size != defaultPagerOptions.Size)
                this.RouteData.Values.Add("pager.size", pager.Size);
            
            // Build view model
            var viewModel = await GetIndexViewModelAsync(opts, pager);

            // Add view model to context
            HttpContext.Items[typeof(TagIndexViewModel<Tag>)] = viewModel;
            
            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetTags", viewModel);
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
                    .Add(S["Tags"]);
            });
            
            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideIndexAsync(new Tag(), this));

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
                    .Add(S["Tags"], tags => tags
                        .Action("Index", "Admin", "Plato.Docs.Tags")
                        .LocalNav())
                    .Add(S["Add Tag"]);
            });
            
            // We need to pass along the featureId
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(new Tag
            {
                FeatureId = await GetFeatureIdAsync()

            }, this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditTagViewModel viewModel)
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
            
            var tag =  new Tag()
            {
                FeatureId = await GetFeatureIdAsync(),
                Name = viewModel.Name,
                Description = viewModel.Description,
                CreatedUserId = user.Id,
                CreatedDate = DateTimeOffset.UtcNow
            };

            var result = await _tagManager.CreateAsync(tag);
            if (result.Succeeded)
            {

                // Indicate new tag so UpdateAsync does not execute within our view provider
                result.Response.IsNewTag = true;

                // Execute view providers
                await _viewProvider.ProvideUpdateAsync(result.Response, this);

                // Add confirmation
                _alerter.Success(T["Tag Added Successfully!"]);

                // Return
                return RedirectToAction(nameof(Index));

            }
            else
            {
                // Report any errors
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
                    .Add(S["Tags"], tags => tags
                        .Action("Index", "Admin", "Plato.Docs.Tags")
                        .LocalNav())
                    .Add(S["Edit Tag"]);
            });
            
            var category = await _tagStore.GetByIdAsync(id);
           
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

            var currentTag = await _tagStore.GetByIdAsync(id);
            if (currentTag == null)
            {
                return NotFound();
            }

            currentTag.ModifiedUserId = user.Id;
            currentTag.ModifiedDate = DateTimeOffset.UtcNow;
            
            var result = await _viewProvider.ProvideUpdateAsync(currentTag, this);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            _alerter.Success(T["Tag Updated Successfully!"]);

            return RedirectToAction(nameof(Index));

        }

        // ------------
        // Delete
        // ------------

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            
            var ok = int.TryParse(id, out int categoryId);
            if (!ok)
            {
                return NotFound();
            }

            var currentTag = await _tagStore.GetByIdAsync(categoryId);

            if (currentTag == null)
            {
                return NotFound();
            }

            var result = await _tagManager.DeleteAsync(currentTag);

            if (result.Succeeded)
            {
                _alerter.Success(T["Tag Deleted Successfully"]);
            }
            else
            {

                _alerter.Danger(T["Could not delete the tag"]);
            }

            return RedirectToAction(nameof(Index));
        }

        // ------------

        async Task<TagIndexViewModel<Tag>> GetIndexViewModelAsync(TagIndexOptions options, PagerOptions pager)
        {
            
            // Get feature
            options.FeatureId = await GetFeatureIdAsync();
        
            if (options.Sort == TagSortBy.Auto)
            {
                options.Sort = TagSortBy.Modified;
                options.Order = OrderBy.Desc;
            }

            // Indicate administrator view
            options.EnableEdit = true;

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            return new TagIndexViewModel<Tag>()
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
