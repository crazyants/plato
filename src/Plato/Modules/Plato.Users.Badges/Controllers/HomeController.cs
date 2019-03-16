using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Layout;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Badges;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Users.Badges.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"
        
        private readonly IViewProviderManager<Badge> _badgeViewProvider;
        private readonly IBreadCrumbManager _breadCrumbManager;
      
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public HomeController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IViewProviderManager<Badge> badgeViewProvider,
            IBreadCrumbManager breadCrumbManager)
        {
            
            _badgeViewProvider = badgeViewProvider;
            _breadCrumbManager = breadCrumbManager;
       
            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        public async Task<IActionResult> Index()
        {
         
            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Badges"]);
            });
            
            // Return view
            return View((LayoutViewModel) await _badgeViewProvider.ProvideIndexAsync(new Badge(), this));

        }
        
        #endregion

    }

}
