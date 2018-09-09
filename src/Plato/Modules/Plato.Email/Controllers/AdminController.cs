using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Email.Models;
using Plato.Email.Stores;
using Plato.Email.ViewModels;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Navigation;

namespace Plato.Email.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IAuthorizationService _authorizationService;
        private readonly IEmailSettingsStore<EmailSettings> _emailSettingsStore;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;
        
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IAuthorizationService authorizationService,
            IEmailSettingsStore<EmailSettings> emailSettingsStore,
            IAlerter alerter,
            IBreadCrumbManager breadCrumbManager)
        {
       
            _alerter = alerter;
            _breadCrumbManager = breadCrumbManager;
            _authorizationService = authorizationService;
            _emailSettingsStore = emailSettingsStore;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        public async Task<IActionResult> Index()
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
                ).Add(S["Settings"], channels => channels
                    .Action("Index", "Admin", "Plato.Settings")
                    .LocalNav()
                ).Add(S["Email Settings"]);
            });


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
                    DefaultFrom = viewModel.SmtpSettings.DefaultFrom,
                    Host = viewModel.SmtpSettings.Host,
                    Port = viewModel.SmtpSettings.Port,
                    UserName = viewModel.SmtpSettings.UserName,
                    Password = viewModel.SmtpSettings.Password,
                    EnableSsl = viewModel.SmtpSettings.EnableSsl,
                    PollingInterval = viewModel.SmtpSettings.PollInterval,
                    BatchSize = viewModel.SmtpSettings.BatchSize,
                    SendAttempts = viewModel.SmtpSettings.SendAttempts,
                    EnablePolling = viewModel.SmtpSettings.EnablePolling
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
                        DefaultFrom = settings.SmtpSettings.DefaultFrom,
                        Host = settings.SmtpSettings.Host,
                        Port = settings.SmtpSettings.Port,
                        UserName = settings.SmtpSettings.UserName,
                        Password = settings.SmtpSettings.Password,
                        EnableSsl = settings.SmtpSettings.EnableSsl,
                        PollInterval = settings.SmtpSettings.PollingInterval,
                        BatchSize = settings.SmtpSettings.BatchSize,
                        SendAttempts = settings.SmtpSettings.SendAttempts,
                        EnablePolling = settings.SmtpSettings.EnablePolling
                    }
                };
            }
            
            // return default settings
            return new EmailSettingsViewModel()
            {
                SmtpSettings = new SmtpSettingsViewModel()
            };

        }


        #endregion


    }
}
