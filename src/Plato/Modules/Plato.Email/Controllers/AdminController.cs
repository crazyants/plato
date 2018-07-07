using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Plato.Email.Models;
using Plato.Email.Stores;
using Plato.Email.ViewModels;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Email.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IAuthorizationService _authorizationService;
        private readonly IEmailSettingsStore<EmailSettings> _emailSettingsStore;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> localizer,
            IAuthorizationService authorizationService,
            IEmailSettingsStore<EmailSettings> emailSettingsStore,
            IAlerter alerter)
        {
       
            _alerter = alerter;
            _authorizationService = authorizationService;
            _emailSettingsStore = emailSettingsStore;

            T = localizer;

        }

        #endregion

        #region "Actions"

        public async Task<IActionResult> Index()
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}
            
            return View(await GetModel());

        }
        

        [HttpPost]
        [ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(EmailSettingsViewModel viewModel)
        {


            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}


            if (!ModelState.IsValid)
            {
                return View(await GetModel());
            }


            var settings = new EmailSettings()
            {
                SmtpSettings = new SmtpSettings()
                {
                    Host = viewModel.SmtpSettings.Host,
                    Port = viewModel.SmtpSettings.Port,
                    UserName = viewModel.SmtpSettings.UserName,
                    Password = viewModel.SmtpSettings.Password
                }
            };
            
            var result = await _emailSettingsStore.SaveAsync(settings);
            if (result != null)
            {
                _alerter.Success(T["Settings Updated Successfully!"]);
            }
            else
            {
                _alerter.Danger(T["A problem occurred updating the settings. Please try again!"]);
            }
            
            return RedirectToAction(nameof(Index));
            
        }
        
        #endregion

        #region "Private Methods"

        private async Task<EmailSettingsViewModel> GetModel()
        {

            var settings = await _emailSettingsStore.GetAsync();
            if (settings != null)
            {
                return new EmailSettingsViewModel()
                {
                    SmtpSettings = new SmtpSettingsViewModel()
                    {
                        Host = settings.SmtpSettings.Host,
                        Port = settings.SmtpSettings.Port,
                        UserName = settings.SmtpSettings.UserName,
                        Password = settings.SmtpSettings.Password
                    }
                };
            }
            
            return new EmailSettingsViewModel();

        }


        #endregion


    }
}
