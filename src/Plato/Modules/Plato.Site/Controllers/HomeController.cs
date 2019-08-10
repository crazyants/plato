using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Site.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"
       
        public HomeController()
        {
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
            return Task.FromResult((IActionResult)View());

        }

        // ---------------------
        // About
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> About()
        {
            // Return view
            return Task.FromResult((IActionResult)View());

        }

        // ---------------------
        // Features
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Features()
        {
            // Return view
            return Task.FromResult((IActionResult)View());

        }


        // ---------------------
        // Modules
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Modules()
        {
            // Return view
            return Task.FromResult((IActionResult)View());

        }


        // ---------------------
        // Pricing
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Pricing()
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
            return Task.FromResult((IActionResult)View());

        }


        #endregion

    }

}
