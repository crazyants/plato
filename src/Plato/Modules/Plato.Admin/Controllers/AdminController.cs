using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation;

namespace Plato.Admin.Controllers
{

    public class AdminController : Controller
    {

        private readonly IBreadCrumbManager _breadCrumbManager;
        
        public IStringLocalizer S { get; }
        
        public AdminController(
            IStringLocalizer<AdminController> stringLocalizer,
            IBreadCrumbManager breadCrumbManager)
        {

            _breadCrumbManager = breadCrumbManager;

            S = stringLocalizer;

        }
        
        public IActionResult Index()
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"]);
            });

            return View();
        }
    }
}
