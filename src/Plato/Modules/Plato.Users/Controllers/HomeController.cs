using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Users.Models;
using Plato.Users.ViewModels;

namespace Plato.Users.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<UserProfile> _userViewProvider;

        public HomeController(
            IViewProviderManager<UserProfile> userViewProvider)
        {
            _userViewProvider = userViewProvider;
        }

        public async Task<IActionResult> Index(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}

            //// Set breadcrumb
            //_breadCrumbManager.Configure(builder =>
            //{
            //    builder.Add(S["Home"], home => home
            //        .Action("Index", "Admin", "Plato.Admin")
            //        .LocalNav()
            //    ).Add(S["Users"]);
            //});

            // default options
            if (filterOptions == null)
            {
                filterOptions = new FilterOptions();
            }

            // default pager
            if (pagerOptions == null)
            {
                pagerOptions = new PagerOptions();
            }

            this.RouteData.Values.Add("page", pagerOptions.Page);

            // Build view
            var result = await _userViewProvider.ProvideIndexAsync(new UserProfile(), this);

            // Return view
            return View(result);

        }


    }
}
