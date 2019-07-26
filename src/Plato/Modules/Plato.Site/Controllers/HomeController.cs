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
        // Discuss
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Discuss()
        {
            // Return view
            return Task.FromResult((IActionResult)View());

        }
        
        #endregion

    }

}
