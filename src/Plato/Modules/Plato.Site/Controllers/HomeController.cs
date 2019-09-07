using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Site.ViewModels;

namespace Plato.Site.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IEmailManager _emailManager;
        private readonly SmtpSettings _smtpSettings;


        public HomeController(
            IEmailManager emailManager,
            IOptions<SmtpSettings> smtpSettings)
        {
            _emailManager = emailManager;
            _smtpSettings = smtpSettings.Value;
        }

        #endregion

        #region "Actions"

        // ---------------------
        // Index / Home
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Index()
        {
            // Return view
            return Task.FromResult((IActionResult) View());

        }

        // ---------------------
        // About
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> About()
        {
            // Return view
            return Task.FromResult((IActionResult) View());

        }

        // ---------------------
        // Features
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Features()
        {
            // Return view
            return Task.FromResult((IActionResult) View());

        }


        // ---------------------
        // Modules
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Modules()
        {
            // Return view
            return Task.FromResult((IActionResult) View());

        }
        
        // ---------------------
        // Pricing
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Pricing()
        {
            // Return view
            return Task.FromResult((IActionResult) View());

        }

        // ---------------------
        // Support Options
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> SupportOptions()
        {
            // Return view
            return Task.FromResult((IActionResult)View());

        }

        // ---------------------
        // Terms
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Terms()
        {
            // Return view
            return Task.FromResult((IActionResult)View());

        }


        // ---------------------
        // Privacy
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Privacy()
        {
            // Return view
            return Task.FromResult((IActionResult)View());

        }



        // ---------------------
        // Contact
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Contact()
        {
            // Return view
            return Task.FromResult((IActionResult) View());

        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactFormViewModel viewModel)
        {

            var body = !string.IsNullOrEmpty(viewModel.Message)
                ? WebUtility.HtmlDecode(viewModel.Message)
                : viewModel.Message;

            // Build message
            var message = new MailMessage
            {
                From = new MailAddress(viewModel.Email),
                Subject = "Plato Feedback Form",
                Body  = body,
                IsBodyHtml = true,
            };

            message.To.Add(_smtpSettings.DefaultFrom);

            // Send message
            var result = await _emailManager.SaveAsync(message);
            if (result.Succeeded)
            {
                // Success - Redirect to confirmation page
                return RedirectToAction(nameof(ContactConfirmation));
            }

            return await Contact(viewModel);

        }

        [HttpGet, AllowAnonymous]
        public IActionResult ContactConfirmation()
        {
            // Return view
            return View();

        }

        #endregion

    }

}
