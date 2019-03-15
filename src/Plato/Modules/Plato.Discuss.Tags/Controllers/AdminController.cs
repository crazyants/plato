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
using Plato.Discuss.Tags.Models;
using Plato.Discuss.Tags.ViewModels;
using Plato.Internal.Data.Abstractions;

namespace Plato.Discuss.Tags.Controllers
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

            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Discuss"], discuss => discuss
                        .Action("Index", "Admin", "Plato.Discuss")
                        .LocalNav())
                    .Add(S["Tags"]);
            });
            
            // Get default options
            var defaultViewOptions = new TagIndexOptions();
            var defaultPagerOptions = new PagerOptions();
            
            // Add non default route data for pagination purposes
            if (opts.Search != defaultViewOptions.Search)
                this.RouteData.Values.Add("opts.search", opts.Search);
            if (opts.TagSort != defaultViewOptions.TagSort)
                this.RouteData.Values.Add("opts.sort", opts.TagSort);
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

            // Return view
            return View(await _viewProvider.ProvideIndexAsync(new Tag(), this));

        }
        
        public async Task<IActionResult> Create()
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Discuss"], discuss => discuss
                        .Action("Index", "Admin", "Plato.Discuss")
                        .LocalNav())
                    .Add(S["Tags"], tags => tags
                        .Action("Index", "Admin", "Plato.Discuss.Tags")
                        .LocalNav())
                    .Add(S["Add Tag"]);
            });
            
            // We need to pass along the featureId
            var model = await _viewProvider.ProvideEditAsync(new Tag
            {
                FeatureId = await GetFeatureIdAsync() 

            }, this);
            return View(model);

        }

        [HttpPost]
        [ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditTagViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            
            var category =  new Tag()
            {
                FeatureId = await GetFeatureIdAsync(),
                Name = viewModel.Name,
                Description = viewModel.Description,
            };

            var result = await _tagManager.CreateAsync(category);
            if (result.Succeeded)
            {

                await _viewProvider.ProvideUpdateAsync(result.Response, this);

                _alerter.Success(T["Tag Added Successfully!"]);

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
                        .LocalNav())
                    .Add(S["Discuss"], discuss => discuss
                        .Action("Index", "Admin", "Plato.Discuss")
                        .LocalNav())
                    .Add(S["Tags"], tags => tags
                        .Action("Index", "Admin", "Plato.Discuss.Tags")
                        .LocalNav())
                    .Add(S["Edit Tag"]);
            });
            
            var category = await _tagStore.GetByIdAsync(id);
            var model = await _viewProvider.ProvideEditAsync(category, this);
            return View(model);

        }

        [HttpPost]
        [ActionName(nameof(Edit))]
        public  async Task<IActionResult> EditPost(int id)
        {

            var currentCategory = await _tagStore.GetByIdAsync(id);
            if (currentCategory == null)
            {
                return NotFound();
            }

            var result = await _viewProvider.ProvideUpdateAsync(currentCategory, this);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            _alerter.Success(T["Tag Updated Successfully!"]);

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

        async Task<TagIndexViewModel<Tag>> GetIndexViewModelAsync(TagIndexOptions options, PagerOptions pager)
        {

            // Get current feature
            var feature = await _featureFacade.GetFeatureByIdAsync(RouteData.Values["area"].ToString());

            // Restrict results to current feature
            if (feature != null)
            {
                options.FeatureId = feature.Id;
            }

            if (options.TagSort == TagSortBy.Auto)
            {
                options.TagSort = TagSortBy.Created;
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
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Tags");
            if (feature != null)
            {
                return feature.Id;
            }

            throw new Exception($"Could not find required feature registration for Plato.Discuss.Tags");
        }
        
    }

}
