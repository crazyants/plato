using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Localization.Extensions;
using Plato.Internal.Models.Users;
using Plato.Users.Services;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class RegisterViewProvider : BaseViewProvider<UserRegistration>
    {

        private readonly IPlatoUserManager<User> _platoUserManager;
        private readonly IContextFacade _contextFacade;
        private readonly IEmailManager _emailManager;
        private readonly ILocaleStore _localeStore;

        public RegisterViewProvider(
            IPlatoUserManager<User> platoUserManager,
            IContextFacade contextFacade, 
            IEmailManager emailManager,
            ILocaleStore localeStore)
        {
            _platoUserManager = platoUserManager;
            _contextFacade = contextFacade;
            _emailManager = emailManager;
            _localeStore = localeStore;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(UserRegistration registration, IViewProviderContext context)
        {

            var viewModel = new RegisterViewModel()
            {
                UserName = registration.UserName,
                Email = registration.Email,
                Password = registration.Password,
                ConfirmPassword = registration.Password
            };

            return Task.FromResult(Views(
                View<RegisterViewModel>("Register.Index.Header", model => viewModel).Zone("header"),
                View<RegisterViewModel>("Register.Index.Content", model => viewModel).Zone("content"),
                View<RegisterViewModel>("Register.Index.Sidebar", model => viewModel).Zone("sidebar"),
                View<RegisterViewModel>("Register.Index.Footer", model => viewModel).Zone("footer")
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(UserRegistration viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(UserRegistration viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<bool> ValidateModelAsync(UserRegistration registration, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new RegisterViewModel
            {
                UserName = registration.UserName,
                Email = registration.Email,
                Password = registration.Password,
                ConfirmPassword = registration.ConfirmPassword
            });
        }

        public override async Task ComposeTypeAsync(UserRegistration registration, IUpdateModel updater)
        {

            var model = new RegisterViewModel()
            {
                UserName = registration.UserName,
                Email = registration.Email,
                Password = registration.Password,
                ConfirmPassword = registration.ConfirmPassword
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {
                registration.UserName = model.UserName;
                registration.Email = model.Email;
                registration.Password = model.Password;
                registration.ConfirmPassword = model.ConfirmPassword;
            }

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(UserRegistration registration, IViewProviderContext context)
        {
       
            // Send account activation email
            var emailConfirmationResult = await _platoUserManager.GetEmailConfirmationUserAsync(registration.UserName);
            if (emailConfirmationResult.Succeeded)
            {
                var updatedUser = emailConfirmationResult.Response;
                if (updatedUser != null)
                {
                    updatedUser.ConfirmationToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(updatedUser.ConfirmationToken));
                    var emailResult = await SendEmailConfirmationTokenAsync(updatedUser);
                    if (!emailResult.Succeeded)
                    {
                        foreach (var error in emailResult.Errors)
                        {
                            context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return await BuildIndexAsync(registration, context);
                    }
                }
            }

            return await BuildIndexAsync(registration, context);

        }

        #region "Private Methods"

        async Task<ICommandResult<EmailMessage>> SendEmailConfirmationTokenAsync(User user)
        {
            
            // Get reset password email
            var culture = await _contextFacade.GetCurrentCultureAsync();
            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, "ConfirmEmail");
            if (email != null)
            {

                // Build email confirmation link
                var baseUrl = await _contextFacade.GetBaseUrlAsync();
                var callbackUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["area"] = "Plato.Users",
                    ["controller"] = "Account",
                    ["action"] = "ActivateAccount",
                    ["code"] = user.ConfirmationToken
                });

                var body = string.Format(email.Message, user.DisplayName, callbackUrl);

                var message = new MailMessage()
                {
                    Subject = email.Subject,
                    Body = WebUtility.HtmlDecode(body),
                    IsBodyHtml = true
                };

                message.To.Add(user.Email);

                // send email
                return await _emailManager.SaveAsync(message);

            }

            var result = new CommandResult<EmailMessage>();
            return result.Failed("An error occurred whilst attempting to send the email confirmation email.");

        }


        #endregion

    }
}
