using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Plato.Users.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Localization.LocaleSerializers;
using Plato.Internal.Models.Users;
using Plato.Users.Services;

namespace Plato.Users.Controllers
{

    public class AccountController : Controller
    {

        private readonly IPlatoUserManager<User> _platoUserManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ILocaleManager _localeManager;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }


        public AccountController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            UserManager<User> userManager,
            SignInManager<User> signInManage,
            ILogger<AccountController> logger, 
            IPlatoUserManager<User> platoUserManager,
            ILocaleManager localeManager)
        {
            _userManager = userManager;
            _signInManager = signInManage;
            _logger = logger;
            _platoUserManager = platoUserManager;
            _localeManager = localeManager;


            T = htmlLocalizer;
            S = stringLocalizer;

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {

            var sb = new StringBuilder();

            sb.Append("<br>")
                .Append("<strong>Email Resources</strong>")
                .Append("<br>");

            var resourceValues1 = await _localeManager.GetResourcesAsync<LocaleEmail>("en-US");
            foreach (var resource in resourceValues1)
            {
                sb.Append("<strong>File Name:</strong> ")
                    .Append(resource.Resource.Name)
                    .Append("<BR>");
                sb.Append("<strong>Location:</strong>")
                    .Append(resource.Resource.Location)
                    .Append("<BR>");

                foreach (var template in resource.Values)
                {
                    sb.Append("To: ").Append(template.To);
                    sb.Append("<BR>");
                    sb.Append("Subject: ").Append(template.Subject);
                    sb.Append("<BR>");
                }
            }

            sb.Append("<br>")
                .Append("-------------------------------------------------")
                .Append("<br>");

            sb.Append("<br>")
                .Append("<strong>String Resources</strong>")
                .Append("<br>");

            var resourceValues = await _localeManager.GetResourcesAsync<LocaleString>("en-US");
            foreach (var resourceValue in resourceValues)
            {
                sb.Append("<strong>File Name:</strong> ")
                    .Append(resourceValue.Resource.Name)
                    .Append("<BR>");
                sb.Append("<strong>Location:</strong>")
                    .Append(resourceValue.Resource.Location)
                    .Append("<BR>");

                foreach (var keyValue in resourceValue.Values)
                {
                    sb.Append("Key: ").Append(keyValue.Key);
                    sb.Append("<BR>");
                    sb.Append("Value: ").Append(keyValue.Value);
                    sb.Append("<BR>");
                }
            }

            sb.Append("<br>")
                .Append("-------------------------------------------------")
                .Append("<br>");


            //sb.Append("<br>")
            //    .Append("<strong>LocaleEmails Resources</strong>")
            //    .Append("<br>");

            //var resources2 = await _localeManager.GetResourcesAsync<LocaleEmails>("en-GB");
            //foreach (var resource in resources2)
            //{
            //    foreach (var template in resource.Templates)
            //    {
            //        sb.Append("To: ").Append(template.To);
            //        sb.Append("<BR>");
            //        sb.Append("Subject: ").Append(template.Subject);
            //        sb.Append("<BR>");
            //    }
            //}

            //sb.Append("<br>")
            //    .Append("-------------------------------------------------")
            //    .Append("<br>");

            
            //var currentLocale = await _localeManager.GetResourcesAsync("en-US");
            //foreach (var resource in currentLocale.Resources.Where(r => r.Type == typeof(LocaleEmails)))
            //{

            //    var emails = (LocaleEmails) resource.Model;

            //    sb.Append("Templates: ").Append(emails.Templates.Count());
            //    sb.Append("<BR>");

            //    foreach (var email in emails.Templates)
            //    {
            //        sb.Append("To: ").Append(email.To);
            //        sb.Append("<BR>");
            //        sb.Append("Subject: ").Append(email.Subject);
            //        sb.Append("<BR>");
            //    }

            //}

            //sb.Append("<br>")
            //    .Append("-------------------------------------------------")
            //    .Append("<br>");


            // -----------


            //    var locales = await _localeManager.GetLocalesAsync();

          
            //foreach (var locale in locales)
            //{

            //    sb
            //        .Append("<strong>Name:</strong> ")
            //        .Append(locale.Descriptor.Name)
            //        .Append("<br>")
            //        .Append("<strong>Path:</strong> ")
            //        .Append(locale.Descriptor.Path);
              
                
            //    foreach (var resource in locale.Resources.Where(r => r.Type == typeof(LocaleEmails)))
            //    {
                    
            //        sb
            //            .Append("<br>")
            //            .Append("<strong>LocaleEmails</strong> ")
            //            .Append("<br>");

            //        var emails = (LocaleEmails) resource.Model;
                  
            //        sb.Append("Templates: ").Append(emails.Templates.Count());
            //        sb.Append("<BR>");

            //        foreach (var email in emails.Templates)
            //        {
            //            sb.Append("To: ").Append(email.To);
            //            sb.Append("<BR>");
            //            sb.Append("Subject: ").Append(email.Subject);
            //            sb.Append("<BR>");
            //        }
                  
            //    }

            //    foreach (var resource in locale.Resources.Where(r => r.Type == typeof(LocaleStrings)))
            //    {

            //        sb
            //            .Append("<br>")
            //            .Append("<strong>LocaleStrings</strong> ")
            //            .Append("<br>");

            //        var kvps = (LocaleStrings)resource.Model;

            //        sb.Append("Locales: ").Append(kvps.KeyValues.Count());
            //        sb.Append("<BR>");

            //        foreach (var kvp in kvps.KeyValues)
            //        {
            //            sb.Append("Key: ").Append(kvp.Key);
            //            sb.Append("<BR>");
            //            sb.Append("Value: ").Append(kvp.Value);
            //            sb.Append("<BR>");
            //        }


            //    }


            //    sb.Append("<hr>");

            //}

            ViewData["Locales"] = sb.ToString();
            
            for (var i = 0; i < 1; i++)
            {

                var displayName = "New User " + i;
                var userNAme = "newuser" + i;
                var email = "email@address" + i + ".com";
                var password = "34Fdckf#343";

                var result = await _platoUserManager.CreateAsync(
                    userNAme,
                    displayName,
                    email,
                    password);

            }

            //var user = _httpContextAccessor.HttpContext.User;
            //var claims = user.Claims;

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            
             // ----------

             ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(
                    model.UserName, 
                    model.Password, 
                    model.RememberMe,
                    lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    //return RedirectToAction(nameof(LoginWith2fa), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    
                    ModelState.AddModelError(string.Empty, "Account Required Two Factor Authentication.");
                    return View(model);

                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    ModelState.AddModelError(string.Empty, "Account Locked out.");
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            
            var model = new RegisterViewModel
            {
                Email = "admin@admin.com",
                UserName = "admin@Adm1in.com",
                Password = "admin@Adm1in.com"
            };

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model, 
            string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    //_logger.LogInformation(3, "User created a new account with password.");
                    return RedirectToLocal(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }

        private IActionResult RedirectToLocal(string returnUrl)
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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
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
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
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
        
    }

}
