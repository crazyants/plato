using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Plato.Abstractions.Data;
using Plato.Abstractions.Query;
using Plato.Layout;
using Plato.Layout.ViewProviders;
using Plato.Layout.Views;
using Plato.Models.Users;
using Plato.Stores.Users;
using Plato.Users.ViewModels;
using Plato.Navigation;
using Plato.Layout.ModelBinding;

namespace Plato.Users.Controllers
{
    
    public class AdminController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<User> _viewProviderManager;
        private readonly IPlatoUserStore<User> _ploatUserStore;
        private readonly UserManager<User> _userManager;

        public AdminController(
            IPlatoUserStore<User> platoUserStore, 
            IViewProviderManager<User> viewProviderManager, 
            UserManager<User> userManager)
        {
            _ploatUserStore = platoUserStore;
            _viewProviderManager = viewProviderManager;
            _userManager = userManager;
        }

        #region "Action Methods"

        public async Task<IActionResult> Index(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {

            // default options
            if (filterOptions == null)
            {
                filterOptions = new FilterOptions();
            }

            if (!string.IsNullOrWhiteSpace(filterOptions.Search))
            {
                //users = users.Where(u => u.NormalizedUserName.Contains(options.Search) || u.NormalizedEmail.Contains(options.Search));
            }

            switch (filterOptions.Order)
            {
                case UsersOrder.Username:
                    //users = users.OrderBy(u => u.NormalizedUserName);
                    break;
                case UsersOrder.Email:
                    //users = users.OrderBy(u => u.NormalizedEmail);
                    break;
            }
            
            // Maintain previous route data when generating page links
            var routeData = new RouteData();
            //routeData.Values.Add("Options.Filter", options.Filter);
            routeData.Values.Add("Options.Search", filterOptions.Search);
            routeData.Values.Add("Options.Order", filterOptions.Order);
            
            var model = await GetPagedModel(filterOptions, pagerOptions);
     
            //var providedView = await _viewProviderManager.BuildDisplayAsync(user, this);

            return View(new View("User.List", model));

        }


        public async Task<IActionResult> LayoutTest(string id)
        {

            var model = new LayoutViewModel();

            return View(model);
        }


        
        public async Task<IActionResult> Display(string id)
        {

            var currentUser = await _userManager.FindByIdAsync(id);
            if (!(currentUser is User))
            {
                return NotFound();
            }

            var result = await _viewProviderManager.ProvideDisplayAsync(currentUser, this);
            return View(result);
        }

        public async Task<IActionResult> Create()
        {
            var result = await _viewProviderManager.ProvideEditAsync(new User(), this);
            return View(result);
        }
        
        public async Task<IActionResult> Edit(string id)
        {

            var currentUser = await _userManager.FindByIdAsync(id);
            if (!(currentUser is User))
            {
                return NotFound();
            }
            
            var result = await _viewProviderManager.ProvideEditAsync(currentUser, this);
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
            
            var result = await _viewProviderManager.ProvideUpdateAsync((User)currentUser, this);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            //_notifier.Success(TH["User updated successfully"]);

            return RedirectToAction(nameof(Index));
            
        }

        #endregion

        #region "Private Methods"

        private async Task<UsersPagedViewModel> GetPagedModel(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            var users = await GetUsers(filterOptions, pagerOptions);
            return new UsersPagedViewModel(
                users,
                filterOptions,
                pagerOptions);
        }

        public async Task<IPagedResults<User>> GetUsers(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            return await _ploatUserStore.QueryAsync()
                .Page(pagerOptions.Page, pagerOptions.PageSize)
                .Select<UserQueryParams>(q =>
                {
                    if (!string.IsNullOrEmpty(filterOptions.Search))
                    {
                        q.UserName.IsIn(filterOptions.Search).Or();
                        q.Email.IsIn(filterOptions.Search);
                    }
                    // q.UserName.IsIn("Admin,Mark").Or();
                    // q.Email.IsIn("email440@address.com,email420@address.com");
                    // q.Id.Between(1, 5);
                })
                .OrderBy("Id", OrderBy.Asc)
                .ToList<User>();
        }


        #endregion

    }
}
