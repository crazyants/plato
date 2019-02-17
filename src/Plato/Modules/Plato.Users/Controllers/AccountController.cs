using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Plato.Users.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Services;

namespace Plato.Users.Controllers
{

    public class AccountController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IUserEmails _userEmails;
        private readonly IPlatoUserManager<User> _platoUserManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IViewProviderManager<UserLogin> _loginViewProvider;
        private readonly IViewProviderManager<UserRegistration> _registerViewProvider;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AccountController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            UserManager<User> userManager,
            SignInManager<User> signInManage,
            ILogger<AccountController> logger, 
            IPlatoUserManager<User> platoUserManager,
            IPlatoUserStore<User> platoUserStore,
            IOptions<IdentityOptions> identityOptions,
            IBreadCrumbManager breadCrumbManager, 
            IViewProviderManager<UserLogin> loginViewProvider,
            IViewProviderManager<UserRegistration> registerViewProvider,
            IAlerter alerter, IUserEmails userEmails)
        {
            _userManager = userManager;
            _signInManager = signInManage;
            _logger = logger;
            _platoUserManager = platoUserManager;
            _platoUserStore = platoUserStore;
            _identityOptions = identityOptions;
            _breadCrumbManager = breadCrumbManager;
            _loginViewProvider = loginViewProvider;
            _registerViewProvider = registerViewProvider;
            _alerter = alerter;
            _userEmails = userEmails;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        // -----------------
        // Login
        // -----------------

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            
            await CreateSampleUsers();

            // ----------------------------------------------------------------

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Login"]);
            });

            // Persist returnUrl
            ViewData["ReturnUrl"] = returnUrl;

            // Build view
            var result = await _loginViewProvider.ProvideIndexAsync(new UserLogin(), this);

            // Return view
            return View(result);
          
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {

            // Persist returnUrl
            ViewData["ReturnUrl"] = returnUrl;

            // Build view provider model
            var userLogin = new UserLogin()
            {
                UserName = model.UserName,
                Password = model.Password,
                RememberMe = model.RememberMe
            };

            // Validate model state within all involved view providers
            if (await _loginViewProvider.IsModelStateValid(userLogin, this))
            {

                // Get composed type from all involved view providers
                var loginViewModel = await _loginViewProvider.GetComposedType(this);

                // Authenticate composed type
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(
                    loginViewModel.UserName,
                    loginViewModel.Password,
                    loginViewModel.RememberMe,
                    lockoutOnFailure: false);

                // Success
                if (result.Succeeded)
                {

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation(1, "User logged in.");
                    }
                    
                    // Execute view providers update method
                    var viewResult = await _loginViewProvider.ProvideUpdateAsync(loginViewModel, this);

                    // No further errors have occurred perform final redirect
                    if (ModelState.ErrorCount == 0)
                    {
                        // Redirect to returnUrl
                        return RedirectToLocal(returnUrl);
                    }

                    // Display errors from Update method
                    return View(viewResult);
                    
                }

                if (result.RequiresTwoFactor)
                {
                    //return RedirectToAction(nameof(LoginWith2fa), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    if (ModelState.ErrorCount == 0)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Account requires two factor authentication.");
                    }
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    if (ModelState.ErrorCount == 0)
                    {
                        ModelState.AddModelError(string.Empty, "Account locked out.");
                    }
                }

                // Inform the user the account requires confirmation
                if (_identityOptions.Value.SignIn.RequireConfirmedEmail)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    if (user != null)
                    {
                        var validPassword = await _userManager.CheckPasswordAsync(user, model.Password);
                        if (validPassword)
                        {
                            // Valid credentials entered
                            if (ModelState.ErrorCount == 0)
                            {
                                ModelState.AddModelError(string.Empty,
                                    "Before you can login you must first confirm your email address. Use the \"Confirm your email address\" link below to resend your account confirmation email.");
                            }
                        }
                    }
                }

                // Invalid login credentials
                if (ModelState.ErrorCount == 0)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }

            }

            // if we reach this point some view model validation
            // failed within a view provider, display model state errors
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    //_alerter.Danger(T[error.ErrorMessage]);
                }
            }

            return await Login(returnUrl);

        }

        // -----------------
        // Register
        // -----------------

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
       
            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Register"]);
            });

            // Add return Url to viewData
            ViewData["ReturnUrl"] = returnUrl;

            var rnd = new Random();
            var email = "email@EmAil" + rnd.Next(0, 10000) + rnd.Next(0, 10000) + ".com";

            // TODO remove
            var model = new UserRegistration()
            {
                UserName = email,
                Email = email,
                Password = "H4s#32ffw1" + rnd.Next(0, 10000),
                ConfirmPassword = "H4s#32ffw1" + rnd.Next(0, 10000)
            };

            // Build view
            var result = await _registerViewProvider.ProvideIndexAsync(model, this);

            // Return view
            return View(result);

        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel viewModel,  string returnUrl = null)
        {

            // Persist returnUrl
            ViewData["ReturnUrl"] = returnUrl;

            // Build model for view providers
            var registration = new UserRegistration()
            {
                UserName = viewModel.UserName,
                Email = viewModel.Email,
                Password = viewModel.Password,
                ConfirmPassword = viewModel.ConfirmPassword
            };

            // Validate model state within all involved view providers
            if (await _registerViewProvider.IsModelStateValid(registration, this))
            {

                // Get composed type from all involved view providers
                var model = await _registerViewProvider.GetComposedType(this);

                // Create the user from composed type
                var result = await _platoUserManager.CreateAsync(model.UserName, model.Email, model.Password);
        
                //var result = await _userManager.CreateAsync(registerViewModel, registerViewModel.Password);
                if (result.Succeeded)
                {
                    
                    // Indicate new flag to allow optional update
                    // on first creation within any involved view provider
                    model.IsNewUser = true;

                    // Execute view providers update method
                    // var viewResult = await _registerViewProvider.ProvideUpdateAsync(registerViewModel, this);
                    await _registerViewProvider.ProvideUpdateAsync(model, this);
                    
                    // Success - Redirect to confirmation page
                    return RedirectToAction(nameof(RegisterConfirmation));

                }
                else
                {
                    // Report errors that may have occurred whilst creating the user
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

            return await Register(returnUrl);

        }

        [HttpGet, AllowAnonymous]
        public IActionResult RegisterConfirmation()
        {

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Register"]);
            });

            return View();
        }
        
        // -----------------
        // Logoff
        // -----------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }
        
        // -----------------
        // Confirm Email
        // -----------------

        [HttpGet, AllowAnonymous]
        public IActionResult ConfirmEmail()
        {

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Confirm Email"]);
            });

            return View(new ConfirmEmailViewModel());
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {

            if (ModelState.IsValid)
            {
                var result = await _platoUserManager.GetEmailConfirmationUserAsync(model.UserIdentifier);
                if (result.Succeeded)
                {
                    var user = result.Response;
                    if (user != null)
                    {
                        user.ConfirmationToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.ConfirmationToken));
                        var emailResult = await _userEmails.SendEmailConfirmationTokenAsync(user);
                        if (!emailResult.Succeeded)
                        {
                            foreach (var error in emailResult.Errors)
                            {
                                ViewData.ModelState.AddModelError(string.Empty, error.Description);
                            }

                            return View(model);
                        }
                    }
                }
            }

            return RedirectToAction(nameof(ConfirmEmailConfirmation));
        }
        
        [HttpGet, AllowAnonymous]
        public IActionResult ConfirmEmailConfirmation()
        {

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Confirm Email"]);
            });

            return View();
        }
        
        // -----------------
        // Activate Account
        // -----------------

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> ActivateAccount(string code = null)
        {
            
            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Activate Account"]);
            });

            var isValidConfirmationToken = false;
            if (!String.IsNullOrEmpty(code))
            {
                if (code.IsBase64String())
                {
                    var user = await _platoUserStore.GetByConfirmationToken(
                        Encoding.UTF8.GetString(Convert.FromBase64String(code)));
                    if (user != null)
                    {
                        isValidConfirmationToken = true;
                    }
                }
            }

            return View(new ActivateAccountViewModel
            {
                IsValidConfirmationToken = isValidConfirmationToken,
                ConfirmationToken = code
            });

        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateAccount(ActivateAccountViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Ensure the user account matches the confirmation token
                    var confirmationToken = Encoding.UTF8.GetString(Convert.FromBase64String(model.ConfirmationToken));
                    if (user.ConfirmationToken == confirmationToken)
                    {
                        var result = await _platoUserManager.ConfirmEmailAsync(model.Email, confirmationToken);
                        if (result.Succeeded)
                        {
                            return RedirectToLocal(Url.Action("ActivateAccountConfirmation"));
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
            }

            // If we reach this point the found user's confirmation token does not match the supplied confirmation code
            ViewData.ModelState.AddModelError(string.Empty, "The email address does not match the confirmation token");
            return await ActivateAccount(model.ConfirmationToken);
        }
        
        [HttpGet, AllowAnonymous]
        public IActionResult ActivateAccountConfirmation()
        {

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Activate Account"]);
            });
            
            return View();

        }
        
        // -----------------
        // Forgot Password
        // -----------------

        [HttpGet, AllowAnonymous]
        public IActionResult ForgotPassword()
        {

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Forgot Password"]);
            });
            
            return View(new ForgotPasswordViewModel());
        }
        
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
          
            if (ModelState.IsValid)
            {
                var result = await _platoUserManager.GetForgotPasswordUserAsync(model.UserIdentifier);
                if (result.Succeeded)
                {
                    var user = result.Response;
                    if (user != null)
                    {
                        // Ensure account has been confirmed
                        if (await _userManager.IsEmailConfirmedAsync(user))
                        {
                            user.ResetToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.ResetToken));
                            var emailResult = await _userEmails.SendPasswordResetTokenAsync(user);
                            if (!emailResult.Succeeded)
                            {
                                foreach (var error in emailResult.Errors)
                                {
                                    ViewData.ModelState.AddModelError(string.Empty, error.Description);
                                }

                                return View(model);
                            }
                        }
                    }
                }
            }

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
            
        }

        [HttpGet, AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Forgot Password"]);
            });

            return View();
        }
        
        // -----------------
        // Reset Password
        // -----------------

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string code = null)
        {

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Reset Password"]);
            });

            // Check token
            var isValidResetToken = false;
            if (!String.IsNullOrEmpty(code))
            {
                if (code.IsBase64String())
                {
                    var user = await _platoUserStore.GetByResetToken(Encoding.UTF8.GetString(Convert.FromBase64String(code)));
                    if (user != null)
                    {
                        isValidResetToken = true;
                    }
                }
            }

            // Return view
            return View(new ResetPasswordViewModel
            {
                IsValidResetToken = isValidResetToken,
                ResetToken = code
            });
        }
        
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
         
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Ensure the user account matches the reset token
                    var resetToken = Encoding.UTF8.GetString(Convert.FromBase64String(model.ResetToken));
                    if (user.ResetToken == resetToken)
                    {
                        var result = await _platoUserManager.ResetPasswordAsync(
                            model.Email,
                            resetToken,
                            model.NewPassword);
                        if (result.Succeeded)
                        {
                            return RedirectToLocal(Url.Action("ResetPasswordConfirmation"));
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
            }

            // If we reach this point the found user's reset token does not match the supplied reset token
            ViewData.ModelState.AddModelError(string.Empty, "The email address does not match the reset token");
            return await ResetPassword(model.ResetToken);
        }

        [HttpGet, AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Reset Password"]);
            });

            return View();
        }
        
        // -----------------
        // Lock out
        // -----------------

        [HttpGet, AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        // -----------------
        // Two factor
        // -----------------

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                }

                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        #endregion

        #region "Private Methods"

        async Task CreateSampleUsers()
        {

            var rnd = new Random();

            var usernames = new string[]
                {
                    "John D",
                    "Mark Dogs",
                    "Reverbe ",
                    "Johan",
                    "jcarreira ",
                    "tokyo2002 ",
                    "ebevernage",
                    "pwelter34",
                    "frankmonroe",
                    "tabs",
                    "johangw",
                    "raymak23",
                    "beats",
                    "Fred",
                    "shan",
                    "scottrudy",
                    "thechop",
                    "lyrog",
                    "daniel.gehr",
                    "Cedrik",
                    "nathanchase",
                    "MattPress",
                    "gert.oelof",
                    "abiniyam",
                    "austinh ",
                    "wasimf",
                    "project.ufa",
                    "einaradolfsen",
                    "bstj",
                    "samos",
                    "jintoppy",
                    "mhelin",
                    "eric-914",
                    "marcus85",
                    "leopetes",
                    "angaler1984",
                    "PeterMull",
                    "Stevie",
                    "coder90",
                    "sharah",
                    "Stephen25",
                    "P4a7ker",
                    "Tipsy",
                    "Ryan",
                    "AndyLivey",
                    "RobertW",
                    "ArronG",
                    "Aleena",
                    "Annie",
                    "Cassie",
                    "Lachlan",
                    "Summers",
                    "Isla",
                    "Greer55",
                    "Carry",
                    "Loulou",
                    "MPatterson",
                    "Padilla",
                    "dejavu1987",
                    "fjanon",
                    "project.ufa",
                    "vraptorche",
                    "appleskin",
                    "jintoppy",
                    "mhelin",
                    "NajiJzr",
                    "eric-914",
                    "cportermo",
                    "jack4it",
                    "sapocockas",
                    "srowan",
                    "atpw25",
                    "ralmlopez",
                    "PartyLineLimo",
                    "murdocj",
                    "unichan2018",
                    "eliemichael",
                    "typedef",
                    "MattEllison",
                    "JaiPundir",
                    "zyberzero",
                    "tim",
                    "zakjan",
                    "revered",
                    "Breaker222",
                    "xenod",
                    "mortenbrudv",
                    "cmd_shell",
                    "mcrose",
                    "cusdom",
                    "recruit-jp",
                    "house",
                    "TedProsoft",
                    "luison",
                    "fritz",
                    "eric",
                    "rossang",
                    "AlDennis",
                    "Oxid2178",
                    "CasiOo",
                    "JimShelly",
                    "cisco",
                    "ToadRage",
                    "ericedgar ",
                    "bryan",
                    "joshuaharderr",
                    "mvehar",
                    "arkadiusz-cholewa",
                    "necipakif",
                    "PeterEltgroth",
                    "redone218",
                    "iiiyx",
                    "seanmill",
                    "00ffx"
                };


            foreach (var username in usernames)
            {
                var displayName = username;
                var userNAme = username;
                var email = username + "@example.com";
                var password = "34Fdckf#343";

                var newUserResult = await _platoUserManager.CreateAsync(new User()
                {
                    UserName = userNAme,
                    Email = email,
                    Password = password,
                    DisplayName = displayName
                });
            }

        }

        IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("~/");
            }
        }
        
     
      
        #endregion

    }

}
