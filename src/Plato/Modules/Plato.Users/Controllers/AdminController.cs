using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Users.ViewModels;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Users.Services;

namespace Plato.Users.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly IUserSecurityStampStore<User> _securityStampStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IPlatoUserManager<User> _platoUserManager;
        private readonly IViewProviderManager<User> _viewProvider;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly UserManager<User> _userManager;
        private readonly IContextFacade _contextFacade;
        private readonly IUserEmails _userEmails;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IUserSecurityStampStore<User> securityStampStore,
            IAuthorizationService authorizationService,
            IPlatoUserManager<User> platoUserManager,
            IViewProviderManager<User> viewProvider,
            IBreadCrumbManager breadCrumbManager,
            UserManager<User> userManager,
            IContextFacade contextFacade,
            IUserEmails userEmails,
            IAlerter alerter)
        {

            _authorizationService = authorizationService;
            _securityStampStore = securityStampStore;
            _breadCrumbManager = breadCrumbManager;
            _platoUserManager = platoUserManager;
            _contextFacade = contextFacade;
            _viewProvider = viewProvider;
            _userManager = userManager;
            _userEmails = userEmails;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #region "Action Methods"

        // --------------
        // Manage Users
        // --------------

        public async Task<IActionResult> Index(int offset, UserIndexOptions opts, PagerOptions pager)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(HttpContext.User, 
                Permissions.ManageUsers))
            {
                return Unauthorized();
            }

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
            if (pager.Size != defaultPagerOptions.Size)
                this.RouteData.Values.Add("pager.size", pager.Size);
            
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

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Users"]);
            });

            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideIndexAsync(new User(), this));

        }
        
        // --------------
        // Create User
        // --------------

        public async Task<IActionResult> Create()
        {

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User, 
                Permissions.AddUsers))
            {
                return Unauthorized();
            }

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
            
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(new User(), this));

        }
        
        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditUserViewModel model)
        {

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User,
                Permissions.AddUsers))
            {
                return Unauthorized();
            }

            // Build user
            var user = new User()
            {
                DisplayName = model.DisplayName,
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password,
                Biography = model.Biography,
                Location = model.Location,
                Signature = model.Signature,
                Url = model.Url
            };

            // Validate model state within view providers
            var valid = await _viewProvider.IsModelStateValidAsync(user, this);

            // Ensure password fields match
            if (model.Password != model.PasswordConfirmation)
            {
                ViewData.ModelState.AddModelError(nameof(model.PasswordConfirmation), "Password and Password Confirmation do not match");
                valid = false;
            }
            
            // Validate model state within all view providers
            if (valid)
            {

                // Get composed model from all involved view providers
                user = await _viewProvider.ComposeModelAsync(user, this);

                // Create the composed type
                var result = await _platoUserManager.CreateAsync(
                    user.UserName,
                    user.DisplayName,
                    user.Email,
                    user.Password);
                if (result.Succeeded)
                {
             
                    // Execute view providers ProvideUpdateAsync method
                    await _viewProvider.ProvideUpdateAsync(result.Response, this);
                 
                    // Everything was OK
                    _alerter.Success(T["User Created Successfully!"]);
                    
                    // Redirect back to edit user
                    return RedirectToAction(nameof(Edit), new RouteValueDictionary()
                    {
                        ["id"] = result.Response.Id.ToString()
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
            
            return await Create();

        }

        // --------------
        // Edit User
        // --------------

        public async Task<IActionResult> Edit(string id)
        {

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User,
                Permissions.EditUsers))
            {
                return Unauthorized();
            }
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
            
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(user, this));

        }
        
        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(string id)
        {

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User,
                Permissions.EditUsers))
            {
                return Unauthorized();
            }

            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Get composed model from view providers
            var  user = await _viewProvider.ComposeModelAsync(existingUser, this);

            // Validate model state within all view providers
            var valid = true; // await _viewProvider.IsModelStateValidAsync(user, this);
            if (valid)
            {
                
                // Update user
                var result = await _platoUserManager.UpdateAsync(user);
                if (result.Succeeded)
                {

                    // Execute view providers ProvideUpdateAsync method
                    await _viewProvider.ProvideUpdateAsync(result.Response, this);

                    // Add confirmation
                    _alerter.Success(T["User Updated Successfully!"]);

                    // Redirect back to edit user
                    return RedirectToAction(nameof(Edit), new RouteValueDictionary()
                    {
                        ["id"] = user.Id.ToString()
                    });

                }
                else
                {
                    // Errors that may have occurred whilst updating the entity
                    foreach (var error in result.Errors)
                    {
                        ViewData.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                
            }

            return await Edit(id);

        }

        // --------------
        // Delete
        // --------------

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User,
                Permissions.DeleteUsers))
            {
                return Unauthorized();
            }
            
            // Ensure the user exists
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            // Delete
            var result = await _platoUserManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _alerter.Success(T["User Deleted Successfully"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }

            return RedirectToAction(nameof(Index));

        }

        // --------------
        // Edit Password
        // --------------

        [HttpGet]
        public async Task<IActionResult> EditPassword(string id)
        {

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User,
                Permissions.ResetUserPasswords))
            {
                return Unauthorized();
            }

            // Get user
            var user = await _userManager.FindByIdAsync(id);

            // Ensure user exists
            if (user == null)
            {
                return NotFound();
            }

            // Add user to the context
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

            // Get reset token
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

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(EditPassword))]
        public async Task<IActionResult> EditPasswordPost(EditPasswordViewModel model)
        {

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User,
                Permissions.ResetUserPasswords))
            {
                return Unauthorized();
            }

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

                        _alerter.Success(T["Password Updated Successfully!"]);

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

        // --------------
        // Manually send account confirmation email
        // --------------

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
                        _alerter.Success(T["Confirmation Email Sent Successfully!"]);
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

        // ------------
        // Manually flag email as confirmed
        // ------------

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
                _alerter.Success(T["User Confirmed Successfully!"]);
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

        // ------------
        // Validate User
        // ------------

        public async Task<IActionResult> ValidateUser(string id)
        {

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User,
                Permissions.UserToVerify))
            {
                return Unauthorized();
            }
            
            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            // Get user we are editing
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

            // Reset staff status
            currentUser.IsStaff = false;
            currentUser.IsStaffUpdatedUserId = 0;
            currentUser.IsStaffUpdatedDate = null;
            
            // Update verified status
            currentUser.IsVerified = true;
            currentUser.IsVerifiedUpdatedUserId = user.Id;
            currentUser.IsVerifiedUpdatedDate = DateTimeOffset.UtcNow;

            // Update user
            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                _alerter.Success(T["User Verified Successfully!"]);
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

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User,
                Permissions.UserToVerify))
            {
                return Unauthorized();
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
                _alerter.Success(T["Verified Status Removed Successfully!"]);
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

        // ------------
        // Staff User
        // ------------

        public async Task<IActionResult> ToStaff(string id)
        {

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User,
                Permissions.UserToStaff))
            {
                return Unauthorized();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            // Get user we are editing
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

            // Reset verified status
            currentUser.IsVerified = false;
            currentUser.IsVerifiedUpdatedUserId = 0;
            currentUser.IsVerifiedUpdatedDate = null;

            // Update staff status
            currentUser.IsStaff = true;
            currentUser.IsStaffUpdatedUserId = user.Id;
            currentUser.IsStaffUpdatedDate = DateTimeOffset.UtcNow;

            // Update user
            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                _alerter.Success(T["User Added To Staff Successfully!"]);
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

        public async Task<IActionResult> FromStaff(string id)
        {

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User,
                Permissions.UserToStaff))
            {
                return Unauthorized();
            }
            
            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            // Get user we are editing
            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                return NotFound();
            }

            // Reset staff status
            currentUser.IsStaff = false;
            currentUser.IsStaffUpdatedUserId = 0;
            currentUser.IsStaffUpdatedDate = null;

            // Update user
            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                _alerter.Success(T["Staff Status Removed Successfully!"]);
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
        
        // ------------
        // Flag As Spam
        // ------------

        public async Task<IActionResult> SpamUser(string id)
        {

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User,
                Permissions.UserToSpam))
            {
                return Unauthorized();
            }


            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            // Get user we are editing
            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                return NotFound();
            }

            // Reset staff status
            currentUser.IsStaff = false;
            currentUser.IsStaffUpdatedUserId = 0;
            currentUser.IsStaffUpdatedDate = null;

            // Reset verified status
            currentUser.IsVerified = false;
            currentUser.IsVerifiedUpdatedUserId = 0;
            currentUser.IsVerifiedUpdatedDate = null;

            // Reset banned status
            currentUser.IsBanned = false;
            currentUser.IsBannedUpdatedUserId = 0;
            currentUser.IsBannedUpdatedDate = null;

            // Update spam status
            currentUser.IsSpam = true;
            currentUser.IsSpamUpdatedUserId = user.Id;
            currentUser.IsSpamUpdatedDate = DateTimeOffset.UtcNow;
            
            // Update user
            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                // Add confirmation
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

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User,
                Permissions.UserToSpam))
            {
                return Unauthorized();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return Unauthorized();
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
                _alerter.Success(T["User Removed from SPAM Successfully!"]);
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

        // ------------
        // Ban User
        // ------------

        public async Task<IActionResult> BanUser(string id)
        {

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User,
                Permissions.UserToBanned))
            {
                return Unauthorized();
            }
            
            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }
            // Get user we are editing
            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                return NotFound();
            }

            // Reset staff status
            currentUser.IsStaff = false;
            currentUser.IsStaffUpdatedUserId = 0;
            currentUser.IsStaffUpdatedDate = null;

            // Reset verified status
            currentUser.IsVerified = false;
            currentUser.IsVerifiedUpdatedUserId = 0;
            currentUser.IsVerifiedUpdatedDate = null;
            
            // Reset spam status
            currentUser.IsSpam = false;
            currentUser.IsSpamUpdatedUserId = 0;
            currentUser.IsSpamUpdatedDate = null;
            
            // Update banned status
            currentUser.IsBanned = true;
            currentUser.IsBannedUpdatedUserId = user.Id;
            currentUser.IsBannedUpdatedDate = DateTimeOffset.UtcNow;

            // Invalidate security stamp to force sign-out banned users
            await _securityStampStore.SetSecurityStampAsync(currentUser, System.Guid.NewGuid().ToString(), new CancellationToken());

            // Update user
            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                // Add confirmation
                _alerter.Success(T["User Banned Successfully!"]);
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
            
            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User,
                Permissions.UserToBanned))
            {
                return Unauthorized();
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
                _alerter.Success(T["User Ban Removed Successfully!"]);
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
