﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Roles;
using Plato.Internal.Navigation;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Internal.Stores.Roles;
using Plato.Roles.ViewModels;

namespace Plato.Roles.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IAuthorizationService _authorizationService;
        private readonly IViewProviderManager<RolesIndexViewModel> _roleIndexViewProvider;
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
            IViewProviderManager<RolesIndexViewModel> roleIndexViewProvider,
            IPlatoRoleStore platoRoleStore, 
            IViewProviderManager<Role> roleViewProvider,
            RoleManager<Role> roleManager, IAlerter alerter,
            IAuthorizationService authorizationService,
            IBreadCrumbManager breadCrumbManager)
        {
            _roleIndexViewProvider = roleIndexViewProvider;
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
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
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
            if (filterOptions == null)
            {
                filterOptions = new FilterOptions();
            }


            // default pager
            if (pagerOptions == null)
            {
                pagerOptions = new PagerOptions();
            }


            // Maintain previous route data when generating page links
            var routeData = new RouteData();
            routeData.Values.Add("Options.RoleName", filterOptions.Search);
            routeData.Values.Add("Options.Order", filterOptions.Order);

            var model = await GetPagedModel(
                filterOptions,
                pagerOptions);
            
            var result = await _roleIndexViewProvider.ProvideIndexAsync(model, this);

            return View(result);

        }
        
        public async Task<IActionResult> Create()
        {

            // Ensuer we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.AddRoles))
            {
                return Unauthorized();
            }
            
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
                    roleClaims.Add(new RoleClaim { ClaimType = Permission.ClaimType, ClaimValue = permissionName });
                }
            }

            role.RoleClaims.RemoveAll(c => c.ClaimType == Permission.ClaimType);
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

        private async Task<RolesIndexViewModel> GetPagedModel(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            var roles = await GetRoles(filterOptions, pagerOptions);
            return new RolesIndexViewModel(
                roles,
                filterOptions,
                pagerOptions);
        }


        public async Task<IPagedResults<Role>> GetRoles(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            return await _platoRoleStore.QueryAsync()
                .Take(pagerOptions.Page, pagerOptions.PageSize)
                .Select<RoleQueryParams>(q =>
                {
                    if (filterOptions.RoleId > 0)
                    {
                        q.Id.Equals(filterOptions.RoleId);
                    }
                    if (!string.IsNullOrEmpty(filterOptions.Search))
                    {
                        q.RoleName.IsIn(filterOptions.Search);
                    }
                })
                .OrderBy("Id", OrderBy.Desc)
                .ToList();
        }

        #endregion
        

    }
}
