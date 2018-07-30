using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Users;
using Plato.Users.ViewModels;
using Plato.Internal.Navigation;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Users.Controllers
{
    
    public class AdminController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<User> _userViewProvider;
        
        private readonly IPlatoUserStore<User> _ploatUserStore;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly UserManager<User> _userManager;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IPlatoUserStore<User> platoUserStore, 
            IViewProviderManager<User> userViewProvider,
            //IViewProviderManager<UsersIndexViewModel> userListViewProvider,
            IBreadCrumbManager breadCrumbManager,
            UserManager<User> userManager,
            IAlerter alerter)
        {

            _ploatUserStore = platoUserStore;
            _userViewProvider = userViewProvider;
            //_userListViewProvider = userListViewProvider;
            _userManager = userManager;
            _breadCrumbManager = breadCrumbManager;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #region "Action Methods"

        public async Task<IActionResult> Index(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
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
                ).Add(S["Users"]);
            });


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

            //// Maintain previous route data when generating page links
            //var routeData = new RouteData();
            //routeData.Values.Add("Options.Search", filterOptions.Search);
            //routeData.Values.Add("Options.Order", filterOptions.Order);

            this.RouteData.Values.Add("page", pagerOptions.Page);


            //var viewModel = new UsersIndexViewModel()
            //{
            //    FilterOpts = filterOptions,
            //    PagerOpts = pagerOptions
            //};


            //// Get model
            //var model = await GetPagedModel(filterOptions, pagerOptions);

            // Build view
            var result = await _userViewProvider.ProvideIndexAsync(new User(), this);

            // Return view
            return View(result);

        }
        
        
        public async Task<IActionResult> Display(string id)
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}


            var currentUser = await _userManager.FindByIdAsync(id);
            if (!(currentUser is User))
            {
                return NotFound();
            }

            var result = await _userViewProvider.ProvideDisplayAsync(currentUser, this);
            return View(result);
        }

        public async Task<IActionResult> Create()
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
                ).Add(S["Users"], channels => channels
                    .Action("Index", "Admin", "Plato.Users")
                    .LocalNav()
                ).Add(S["Add User"]);
            });

            var result = await _userViewProvider.ProvideEditAsync(new User(), this);
            return View(result);
        }
        
        public async Task<IActionResult> Edit(string id)
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
                ).Add(S["Users"], channels => channels
                    .Action("Index", "Admin", "Plato.Users")
                    .LocalNav()
                ).Add(S["Edit User"]);
            });

            var currentUser = await _userManager.FindByIdAsync(id);
            if (!(currentUser is User))
            {
                return NotFound();
            }
            
            var result = await _userViewProvider.ProvideEditAsync(currentUser, this);
            return View(result);

        }
        
        [HttpPost]
        [ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                return NotFound();
            }
            
            var result = await _userViewProvider.ProvideUpdateAsync((User)currentUser, this);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            _alerter.Success(T["User Updated Successfully!"]);

            return RedirectToAction(nameof(Index));
            
        }

        #endregion

        #region "Private Methods"

        //private async Task<UsersIndexViewModel> GetPagedModel(
        //    FilterOptions filterOptions,
        //    PagerOptions pagerOptions)
        //{
        //    var users = await GetUsers(filterOptions, pagerOptions);
        //    return new UsersIndexViewModel(
        //        users,
        //        filterOptions,
        //        pagerOptions);
        //}

        //public async Task<IPagedResults<User>> GetUsers(
        //    FilterOptions filterOptions,
        //    PagerOptions pagerOptions)
        //{
        //    return await _ploatUserStore.QueryAsync()
        //        .Take(pagerOptions.Page, pagerOptions.PageSize)
        //        .Select<UserQueryParams>(q =>
        //        {
        //            if (!string.IsNullOrEmpty(filterOptions.Search))
        //            {
        //                q.UserName.IsIn(filterOptions.Search).Or();
        //                q.Email.IsIn(filterOptions.Search);
        //            }
        //            // q.UserName.IsIn("Admin,Mark").Or();
        //            // q.Email.IsIn("email440@address.com,email420@address.com");
        //            // q.Id.Between(1, 5);
        //        })
        //        .OrderBy("Id", OrderBy.Asc)
        //        .ToList();
        //}
        
        #endregion

    }
}
