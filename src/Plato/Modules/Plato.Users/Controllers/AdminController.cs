using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Users.ViewModels;
using Plato.Internal.Navigation;
using Plato.Internal.Layout.ModelBinding;
using Plato.Users.Services;

namespace Plato.Users.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<User> _viewProvider;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly UserManager<User> _userManager;
        private readonly IPlatoUserManager<User> _platoUserManager;
        private readonly IAlerter _alerter;
        private readonly IContextFacade _contextFacade;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IViewProviderManager<User> viewProvider,
            IBreadCrumbManager breadCrumbManager,
            UserManager<User> userManager,
            IAlerter alerter,
            IPlatoUserManager<User> platoUserManager,
            IContextFacade contextFacade)
        {
            _viewProvider = viewProvider;
            _userManager = userManager;
            _breadCrumbManager = breadCrumbManager;
            _alerter = alerter;
            _platoUserManager = platoUserManager;
            _contextFacade = contextFacade;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #region "Action Methods"

        public async Task<IActionResult> Index(
            int offset,
            UserIndexOptions opts,
            PagerOptions pager)
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}

            // Set breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Users"]);
            });
            
            // default options
            if (opts == null)
            {
                opts = new UserIndexOptions();
            }

            // default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            if (offset > 0)
            {
                pager.Page = offset.ToSafeCeilingDivision(pager.PageSize);
                pager.SelectedOffset = offset;
            }

            // Get default options
            var defaultViewOptions = new UserIndexOptions();
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
            opts.Scroller = new ScrollerOptions
            {
                Url = GetInfiniteScrollCallbackUrl()
            };

            // Enable edit options for admin view
            opts.EnableEdit = true;

            var viewModel = new UserIndexViewModel()
            {
                Options = opts,
                Pager = pager
            };

            // Add view options to context for use within view adaptors
            HttpContext.Items[typeof(UserIndexViewModel)] = viewModel;

            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetUsers", viewModel);
            }

            // Build view
            var result = await _viewProvider.ProvideIndexAsync(new User(), this);

            // Return view
            return View(result);

        }

        [HttpGet]
        public async Task<IActionResult> Display(string id)
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}


            var currentUser = await _userManager.FindByIdAsync(id);
            if (!(currentUser is User))
            {
                return NotFound();
            }

            var result = await _viewProvider.ProvideDisplayAsync(currentUser, this);
            return View(result);
        }

        public async Task<IActionResult> Create()
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}


            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Users"], channels => channels
                    .Action("Index", "Admin", "Plato.Users")
                    .LocalNav()
                ).Add(S["Add User"]);
            });

            var result = await _viewProvider.ProvideEditAsync(new User(), this);
            return View(result);
        }

        [HttpPost]
        [ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditUserViewModel model)
        {

            // Validate model state within view providers
            var valid = await _viewProvider.IsModelStateValid(new User()
            {
                DisplayName = model.DisplayName,
                UserName = model.UserName,
                Email = model.Email,
            }, this);

            // Ensure password fields match
            if (model.Password != model.PasswordConfirmation)
            {
                ViewData.ModelState.AddModelError(nameof(model.PasswordConfirmation), "Password and Password Confirmation do not match");
                valid = false;
            }
            
            // Validate model state within all view providers
            if (valid)
            {
             
                // Get fully composed type from all involved view providers
                //var user = await _viewProvider.GetComposedType(this);

                // We need to first add the fully composed type
                // so we have a nuique Id for all ProvideUpdateAsync
                // methods within any involved view provider
                var result = await _platoUserManager.CreateAsync(
                    model.UserName,
                    model.DisplayName,
                    model.Email,
                    model.Password);

                if (result.Succeeded)
                {

                    // Get new user
                    var newUser = await _userManager.FindByEmailAsync(model.Email);
                    if (newUser != null)
                    {
                        // Execute view providers ProvideUpdateAsync method
                        await _viewProvider.ProvideUpdateAsync(newUser, this);
                    }
                    
                    // Everything was OK
                    _alerter.Success(T["User Created Successfully!"]);

                    // Redirect to edit user
                    // Redirect back to edit user
                    return RedirectToAction(nameof(Edit), new RouteValueDictionary()
                    {
                        ["id"] = newUser.Id.ToString()
                    });

                }
                else
                {
                    // Errors that may have occurred whilst creating the entity
                    foreach (var error in result.Errors)
                    {
                        ViewData.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                
            }
            
            // if we reach this point some view model validation
            // failed within a view provider, display model state errors
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _alerter.Danger(T[error.ErrorMessage]);
                }
            }

            return await Create();

        }
        
        public async Task<IActionResult> Edit(string id)
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}


            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Users"], channels => channels
                    .Action("Index", "Admin", "Plato.Users")
                    .LocalNav()
                ).Add(S["Edit User"]);
            });

            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                return NotFound();
            }
            
            var result = await _viewProvider.ProvideEditAsync(currentUser, this);
            return View(result);

        }
        
        [HttpPost]
        [ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                return NotFound();
            }
            
            var result = await _viewProvider.ProvideUpdateAsync((User)currentUser, this);
            if (ModelState.IsValid)
            {

                _alerter.Success(T["User Updated Successfully!"]);

                // Redirect back to edit user
                return RedirectToAction(nameof(Edit), new RouteValueDictionary()
                {
                    ["id"] = currentUser.Id.ToString()
                });

            }

            // if we reach this point some view model validation
            // failed within a view provider, display model state errors
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _alerter.Danger(T[error.ErrorMessage]);
                }
            }

            return await Edit(currentUser.Id.ToString());

        }

        public async Task<IActionResult> ConfirmEmail(string id)
        {
            // Get user
            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                return NotFound();
            }

            currentUser.EmailConfirmed = true;

            // Update user
            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                _alerter.Success(T["User activated Successfully!"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }
            
            // Redirect back to edit user
            return RedirectToAction(nameof(Edit), new RouteValueDictionary()
            {
                ["id"] = id
            });
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
