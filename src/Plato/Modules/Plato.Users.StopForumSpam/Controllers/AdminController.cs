using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Scripting.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.Users.StopForumSpam.Models;
using Plato.Users.StopForumSpam.Stores;
using Plato.Users.StopForumSpam.ViewModels;

namespace Plato.Users.StopForumSpam.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {

        private readonly IAuthorizationService _authorizationService;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IViewProviderManager<StopForumSpamSettings> _viewProvider;
        private readonly IStopForumSpamSettingsStore<StopForumSpamSettings> _recaptchaSettingsStore;
        private readonly IScriptManager _scriptManager;
        private readonly IContextFacade _contextFacade;

        private readonly IStopForumSpamChecker _stopForumSpamChecker;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IAuthorizationService authorizationService,
            IAlerter alerter, 
            IBreadCrumbManager breadCrumbManager,
            IViewProviderManager<StopForumSpamSettings> viewProvider,
            IStopForumSpamSettingsStore<StopForumSpamSettings> recaptchaSettingsStore,
            IStopForumSpamChecker stopForumSpamChecker, 
            IScriptManager scriptManager,
            IContextFacade contextFacade)
        {
            _authorizationService = authorizationService;
            _alerter = alerter;
            _breadCrumbManager = breadCrumbManager;
            _viewProvider = viewProvider;
            _recaptchaSettingsStore = recaptchaSettingsStore;
            _stopForumSpamChecker = stopForumSpamChecker;
            _scriptManager = scriptManager;
            _contextFacade = contextFacade;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<IActionResult> Index()
        {
            
            // Register bootstrap slider client script
            //var baseUri = await _contextFacade.GetBaseUrlAsync();
            //_scriptManager.RegisterScriptBlock(new ScriptBlock(new Dictionary<string, object>()
            //{
            //    ["src"] = "/plato.users.stopforumspam/content/js/bootstrap-slider.js"
            //}), ScriptSection.Footer);
        

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Settings"], channels => channels
                    .Action("Index", "Admin", "Plato.Settings")
                    .LocalNav()
                ).Add(S["StopForumSpam"]);
            });

            // Get StopForumSpam settings
            var settings = await _recaptchaSettingsStore.GetAsync();

            // Build view
            var result = await _viewProvider.ProvideEditAsync(new StopForumSpamSettings()
            {
                ApiKey = settings?.ApiKey ?? "",
                SpamLevel = settings?.SpamLevel ?? 0
            }, this);
            
            var user = new User()
            {
                UserName = "RobertUserm",
                Email = "temptest953305220@gmail.com",
                IpV4Address = "3.16.155.194"
            };

            // Configure checker
            _stopForumSpamChecker.Configure(o =>
            {
                o.ApiKey = settings?.ApiKey ?? "";
            });

            // Get frequencies
            var frequencies = await _stopForumSpamChecker.CheckAsync(user);
            
            var sb = new StringBuilder();

            sb
                .Append("Username: ")
                .Append(frequencies.UserName.Count)
                .Append(", ")
                .Append("Email: ")
                .Append(frequencies.Email.Count)
                .Append(", ")
                .Append("Ip: ")
                .Append(frequencies.IpAddress.Count)
                .Append(", ")
                .Append("success: ")
                .Append(frequencies.Success)
                .Append(", ");

            ViewData["Test"] = sb.ToString();
            



            //Return view
            return View(result);

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(StopForumSpamSettingsViewModel viewModel)
        {

            // Execute view providers ProvideUpdateAsync method
            await _viewProvider.ProvideUpdateAsync(new StopForumSpamSettings()
            {
                ApiKey = viewModel.ApiKey
            }, this);

            // Add alert
            _alerter.Success(T["Settings Updated Successfully!"]);

            return RedirectToAction(nameof(Index));

        }



     



    }

}
