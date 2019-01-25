using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Roles;
using Plato.Internal.Navigation;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Roles.ViewModels;

namespace Plato.Roles.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IAuthorizationService _authorizationService;

        private readonly IViewProviderManager<Role> _roleViewProvider;
        private readonly IPlatoRoleStore _platoRoleStore;
        private readonly RoleManager<Role> _roleManager;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }


        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IPlatoRoleStore platoRoleStore, 
            IViewProviderManager<Role> roleViewProvider,
            RoleManager<Role> roleManager, IAlerter alerter,
            IAuthorizationService authorizationService,
            IBreadCrumbManager breadCrumbManager)
        {
        
            _platoRoleStore = platoRoleStore;
            _roleViewProvider = roleViewProvider;
            _roleManager = roleManager;
            _alerter = alerter;
            _authorizationService = authorizationService;
            _breadCrumbManager = breadCrumbManager;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        public async Task<IActionResult> Index(
            RoleIndexOptions opts,
            PagerOptions pager)
        {
            
            // Ensuer we have permission
            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageRoles))
            //{
            //    return Unauthorized();
            //}

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Roles"]);
            });
            
            // default options
            if (opts == null)
            {
                opts = new RoleIndexOptions();
            }


            // default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }


            // Get default options
            var defaultViewOptions = new RoleIndexOptions();
            var defaultPagerOptions = new PagerOptions();

            // Add non default route data for pagination purposes
            if (opts.Search != defaultViewOptions.Search)
                this.RouteData.Values.Add("opts.search", opts.Search);
            if (opts.Sort != defaultViewOptions.Sort)
                this.RouteData.Values.Add("opts.sort", opts.Sort);
            if (opts.Order != defaultViewOptions.Order)
                this.RouteData.Values.Add("opts.order", opts.Order);
            if (pager.Page != defaultPagerOptions.Page)
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.PageSize != defaultPagerOptions.PageSize)
                this.RouteData.Values.Add("pager.size", pager.PageSize);
            
            //// Maintain previous route data when generating page links
            //var routeData = new RouteData();
            //routeData.Values.Add("opts.search", opts.Search);
            //routeData.Values.Add("opts.order", opts.Order);
            
            // Add view model to context for involved view providers
            this.HttpContext.Items[typeof(RolesIndexViewModel)] = new RolesIndexViewModel()
            {
                Options = opts,
                Pager = pager
            };

            var result = await _roleViewProvider.ProvideIndexAsync(new Role(), this);

            return View(result);

        }
        
        public async Task<IActionResult> Create()
        {

            // Ensuer we have permission
            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.AddRoles))
            //{
            //    return Unauthorized();
            //}
            
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Roles"], channels => channels
                    .Action("Index", "Admin", "Plato.Roles")
                    .LocalNav()
                ).Add(S["Add Role"]);
            });

            var result = await _roleViewProvider.ProvideEditAsync(new Role(), this);
            return View(result);
        }

        [HttpPost]
        [ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost()
        {
            
            var result = await _roleViewProvider.ProvideUpdateAsync(new Role(), this);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            _alerter.Success(T["Role Created Successfully!"]);

            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Edit(string id)
        {

            // Ensuer we have permission
            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditRoles))
            //{
            //    return Unauthorized();
            //}
            
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Roles"], channels => channels
                    .Action("Index", "Admin", "Plato.Roles")
                    .LocalNav()
                ).Add(S["Edit Role"]);
            });

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var result = await _roleViewProvider.ProvideEditAsync(role, this);
            return View(result);

        }
        
        [HttpPost]
        [ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(string id)
        {
            
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            
            var roleClaims = new List<RoleClaim>();
            foreach (string key in Request.Form.Keys)
            {
                if (key.StartsWith("Checkbox.") && Request.Form[key] == "true")
                {
                    var permissionName = key.Substring("Checkbox.".Length);
                    roleClaims.Add(new RoleClaim { ClaimType = Permission.ClaimTypeName, ClaimValue = permissionName });
                }
            }

            role.RoleClaims.RemoveAll(c => c.ClaimType == Permission.ClaimTypeName);
            role.RoleClaims.AddRange(roleClaims);
            
            var result = await _roleViewProvider.ProvideUpdateAsync(role, this);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            _alerter.Success(T["Role Updated Successfully!"]);

            return RedirectToAction(nameof(Index));
            
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            
            // Ensuer we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.DeleteRoles))
            {
                return Unauthorized();
            }
            
            // Ensure we found the role to delete
            var currentRole = await _roleManager.FindByIdAsync(id);
            if (currentRole == null)
            {
                return NotFound();
            }

            // Attempt to delete the role
            var result = await _roleManager.DeleteAsync(currentRole);
            if (result.Succeeded)
            {
                _alerter.Success(T["Role Deleted Successfully"]);
            }
            else
            {

                _alerter.Danger(T["Could not delete the role"]);
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }

            return RedirectToAction(nameof(Index));

        }
        
        #endregion

        #region "Private Methods"


        #endregion
        

    }
}
