using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Users.Services;

namespace Plato.Users.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly IUserEmails _userEmails;
        private readonly IAuthorizationService _authorizationService;
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
            IContextFacade contextFacade,
            IAuthorizationService authorizationService,
            IUserEmails userEmails)
        {
            _viewProvider = viewProvider;
            _userManager = userManager;
            _breadCrumbManager = breadCrumbManager;
            _alerter = alerter;
            _platoUserManager = platoUserManager;
            _contextFacade = contextFacade;
            _authorizationService = authorizationService;
            _userEmails = userEmails;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #region "Action Methods"

        public async Task<IActionResult> Index(
            int offset,
            UserIndexOptions opts,
            PagerOptions pager)
        {

            var claims = "";
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user?.UserRoles != null)
            {
                foreach (var role in user.UserRoles)
                {

                    foreach (var claim in role.RoleClaims)
                    {
                        //if (claim.Type == ClaimTypes.Role)
                        //{
                        claims += claim.ClaimType + " - " + claim.ClaimValue + "<br>";
                        //}
                    }

                }

            }


            claims += "<br><br>----------<br><br>";

            foreach (var claim in HttpContext.User.Claims)
            {
                //if (claim.Type == ClaimTypes.Role)
                //{
                    claims += claim.Type + " - " + claim.Value + "<br>";
                //}
            }

            ViewData["claims"] = claims;

            //if (!await _authorizationService.AuthorizeAsync<Permission>(HttpContext.User, Permissions.ManageUsers))
            //{
            //    return Unauthorized();
            //}

            // Build breadcrumb
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

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

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
            
            // Enable edit options for admin view
            opts.EnableEdit = true;

            var viewModel = new UserIndexViewModel()
            {
                Options = opts,
                Pager = pager
            };

            // Add view options to context for use within view adapters
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
            
            // Ensure user exists
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Add user we are editing to context
            HttpContext.Items[typeof(User)] = user;

            // Build view
            var result = await _viewProvider.ProvideDisplayAsync(user, this);
            return View(result);

        }

        // --------------
        // Create User
        // --------------

        public async Task<IActionResult> Create()
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}

            // Build breadcrumb
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
        
        [HttpPost, ActionName(nameof(Create))]
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

        // --------------
        // Edit User
        // --------------

        public async Task<IActionResult> Edit(string id)
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}
       
            // Ensure user exists
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Add user we are editing to context
            HttpContext.Items[typeof(User)] = user;

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Users"], channels => channels
                    .Action("Index", "Admin", "Plato.Users")
                    .LocalNav()
                ).Add(S[user.DisplayName]);
            });

            var result = await _viewProvider.ProvideEditAsync(user, this);
            return View(result);

        }
        
        [HttpPost, ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(string id)
        {

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            var result = await _viewProvider.ProvideUpdateAsync((User)user, this);
            if (ModelState.IsValid)
            {

                _alerter.Success(T["User Updated Successfully!"]);

                // Redirect back to edit user
                return RedirectToAction(nameof(Edit), new RouteValueDictionary()
                {
                    ["id"] = user.Id.ToString()
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

            return await Edit(user.Id.ToString());

        }

        // --------------
        // Edit Password
        // --------------

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> EditPassword(string id)
        {

            // Ensure user exists
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Add user we are editing to context
            HttpContext.Items[typeof(User)] = user;

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav()
                    ).Add(S["Users"], channels => channels
                        .Action("Index", "Admin", "Plato.Users")
                        .LocalNav()
                    ).Add(S[user.DisplayName], channels => channels
                        .Action("Edit", "Admin", "Plato.Users", new RouteValueDictionary()
                        {
                            ["Id"] = user.Id.ToString()
                        })
                        .LocalNav()
                    )
                    .Add(S["Edit Password"]);
            });

            // Apply reset token
            var resetToken = "";
            var result = await _platoUserManager.GetForgotPasswordUserAsync(user.Email);
            if (result.Succeeded)
            {
                if (result.Response != null)
                {
                    resetToken = result.Response.ResetToken;
                }
            }
            
            // Return view
            return View(new EditPasswordViewModel
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                ResetToken = resetToken
            });

        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPassword(EditPasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    var result = await _platoUserManager.ResetPasswordAsync(
                        model.Email,
                        model.ResetToken,
                        model.NewPassword);
                    if (result.Succeeded)
                    {

                        _alerter.Success(T["Password updated successfully!"]);

                        // Redirect back to edit user
                        return RedirectToAction(nameof(Edit), new RouteValueDictionary()
                        {
                            ["id"] = user.Id.ToString()
                        });
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ViewData.ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }

            // If we reach this point the found user's reset token does not match the supplied reset token
            return await EditPassword(model.Id);
        }

        // -------------------------
        // Various helper actions 
        // -------------------------

        public async Task<IActionResult> ResendConfirmationEmail(string id)
        {

            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                return NotFound();
            }
            
            // Resend confirmation email
            var result = await _platoUserManager.GetEmailConfirmationUserAsync(currentUser.Email);
            if (result.Succeeded)
            {
                var user = result.Response;
                if (user != null)
                {
                    user.ConfirmationToken = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(user.ConfirmationToken));
                    var emailResult = await _userEmails.SendEmailConfirmationTokenAsync(user);
                    if (result.Succeeded)
                    {
                        _alerter.Success(T["Confirmation email sent successfully!"]);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            _alerter.Danger(T[error.Description]);
                        }
                    }
                    
                }
            }

            // Redirect back to edit user
            return RedirectToAction(nameof(Edit), new RouteValueDictionary()
            {
                ["id"] = id
            });
            
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
                _alerter.Success(T["User confirmed successfully!"]);
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

        public async Task<IActionResult> ValidateUser(string id)
        {

            // We need to be authenticated
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            // We need to be an administrator 
            if (!user.RoleNames.Contains(DefaultRoles.Administrator))
            {
                return NotFound();
            }

            // Get user
            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                return NotFound();
            }

            // Reset spam status
            currentUser.IsSpam = false;
            currentUser.IsSpamUpdatedUserId = 0;
            currentUser.IsSpamUpdatedDate = null;

            // Reset banned status
            currentUser.IsBanned = false;
            currentUser.IsBannedUpdatedUserId = 0;
            currentUser.IsBannedUpdatedDate = null;
            
            // Update verified status
            currentUser.IsVerified = true;
            currentUser.IsVerifiedUpdatedUserId = user.Id;
            currentUser.IsVerifiedUpdatedDate = DateTimeOffset.UtcNow;

            // Update user
            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                _alerter.Success(T["User verified successfully!"]);
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

        public async Task<IActionResult> InvalidateUser(string id)
        {

            // We need to be authenticated
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            // We need to be an administrator 
            if (!user.RoleNames.Contains(DefaultRoles.Administrator))
            {
                return NotFound();
            }

            // Get user
            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                return NotFound();
            }
            
            // Reset verified status
            currentUser.IsVerified = false;
            currentUser.IsVerifiedUpdatedUserId = 0;
            currentUser.IsVerifiedUpdatedDate = null;

            // Update user
            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                _alerter.Success(T["Verified status removed successfully!"]);
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
        
        public async Task<IActionResult> BanUser(string id)
        {

            // We need to be authenticated
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            // We need to be an administrator 
            if (!user.RoleNames.Contains(DefaultRoles.Administrator))
            {
                return NotFound();
            }

            // Get user
            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                return NotFound();
            }

            // Reset verified status
            currentUser.IsVerified = false;
            currentUser.IsVerifiedUpdatedUserId = 0;
            currentUser.IsVerifiedUpdatedDate = null;

            // Update banned status
            currentUser.IsBanned = true;
            currentUser.IsBannedUpdatedUserId = user.Id;
            currentUser.IsBannedUpdatedDate = DateTimeOffset.UtcNow;
            
            // Update user
            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                _alerter.Success(T["User added to banned successfully!"]);
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

        public async Task<IActionResult> RemoveBan(string id)
        {

            // We need to be authenticated
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            // We need to be an administrator 
            if (!user.RoleNames.Contains(DefaultRoles.Administrator))
            {
                return NotFound();
            }

            // Get user
            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                return NotFound();
            }
            
            // Reset banned status
            currentUser.IsBanned = false;
            currentUser.IsBannedUpdatedUserId = 0;
            currentUser.IsBannedUpdatedDate = null;

            // Update user
            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                _alerter.Success(T["User removed from banned successfully!"]);
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

        public async Task<IActionResult> SpamUser(string id)
        {

            // We need to be authenticated
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            // We need to be an administrator 
            if (!user.RoleNames.Contains(DefaultRoles.Administrator))
            {
                return NotFound();
            }

            // Get user
            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                return NotFound();
            }
            
            // Update spam status
            currentUser.IsSpam = true;
            currentUser.IsSpamUpdatedUserId = user.Id;
            currentUser.IsSpamUpdatedDate = DateTimeOffset.UtcNow; 
            
            // Update user
            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                _alerter.Success(T["User added to SPAM successfully!"]);
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

        public async Task<IActionResult> RemoveSpam(string id)
        {

            // We need to be authenticated
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            // We need to be an administrator 
            if (!user.RoleNames.Contains(DefaultRoles.Administrator))
            {
                return NotFound();
            }

            // Get user
            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                return NotFound();
            }

            // Reset spam status
            currentUser.IsSpam = false;
            currentUser.IsSpamUpdatedUserId = 0;
            currentUser.IsSpamUpdatedDate = null;
            
            // Update user
            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                _alerter.Success(T["User removed from SPAM successfully!"]);
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
        
    }

}
