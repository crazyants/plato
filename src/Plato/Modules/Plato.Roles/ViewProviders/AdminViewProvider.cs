using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Roles;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Internal.Stores.Roles;
using Plato.Roles.ViewModels;

namespace Plato.Roles.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<Role>
    {

        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IPlatoRoleStore _platoRoleStore;
        private readonly IPermissionsManager<Permission> _permissionsManager;
        private readonly IAuthorizationService _authorizationService;

        public AdminViewProvider(
            UserManager<User> userManager,
            IPlatoRoleStore platoRoleStore,
            RoleManager<Role> roleManager,
            IPermissionsManager<Permission> permissionsManager, 
            IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _platoRoleStore = platoRoleStore;
            _roleManager = roleManager;
            _permissionsManager = permissionsManager;
            _authorizationService = authorizationService;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildDisplayAsync(Role role, IViewProviderContext updater)
        {

            return Task.FromResult(
                Views(
                    View<Role>("Admin.Display.Header", model => role).Zone("header"),
                    View<Role>("Admin.Display.Meta", model => role).Zone("meta"),
                    View<Role>("Admin.Display.Content", model => role).Zone("content"),
                    View<Role>("Admin.Display.Footer", model => role).Zone("footer")
                ));

        }

        public override async Task<IViewProviderResult> BuildIndexAsync(Role role, IViewProviderContext context)
        {

            var userIndexViewModel = context.Controller.HttpContext.Items[typeof(RolesIndexViewModel)] as RolesIndexViewModel;
            
            var viewModel = await GetPagedModel(
                userIndexViewModel?.Options,
                userIndexViewModel?.Pager);

            return Views(
                View<RolesIndexViewModel>("Admin.Index.Header", model => viewModel).Zone("header"),
                View<RolesIndexViewModel>("Admin.Index.Tools", model => viewModel).Zone("tools"),
                View<RolesIndexViewModel>("Admin.Index.Content", model => viewModel).Zone("content")
            );

        }

        public override async Task<IViewProviderResult> BuildEditAsync(Role role, IViewProviderContext updater)
        {
        
            var editRoleViewModel = new EditRoleViewModel()
            {
                Id = role.Id,
                RoleName = role.Name,
                Role = role,
                IsNewRole = await IsNewRole(role.Id),
                EnabledPermissions = await GetEnabledRolePermissionsAsync(role),
                CategorizedPermissions = await _permissionsManager.GetCategorizedPermissionsAsync()
            };

            return Views(
                View<EditRoleViewModel>("Admin.Edit.Header", model => editRoleViewModel).Zone("header"),
                View<EditRoleViewModel>("Admin.Edit.Meta", model => editRoleViewModel).Zone("meta"),
                View<EditRoleViewModel>("Admin.Edit.Content", model => editRoleViewModel).Zone("content"),
                View<EditRoleViewModel>("Admin.Edit.Footer", model => editRoleViewModel).Zone("footer"),
                View<EditRoleViewModel>("Admin.Edit.Actions", model => editRoleViewModel).Zone("actions")
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Role role, IViewProviderContext context)
        {

            var model = new EditRoleViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(role, context);
            }

            if (context.Updater.ModelState.IsValid)
            {

                role.Name = model.RoleName?.Trim();
                
                var result = await _roleManager.CreateAsync(role);
                foreach (var error in result.Errors)
                {
                    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return await BuildEditAsync(role, context);

        }

        #endregion

        #region "Private Methods"

        async Task<bool> IsNewRole(int roleId)
        {
            return await _roleManager.FindByIdAsync(roleId.ToString()) == null;
        }
        
        async Task<RolesIndexViewModel> GetPagedModel(
            RoleIndexOptions options,
            PagerOptions pager)
        {
            var roles = await GetRoles(options, pager);
            return new RolesIndexViewModel(
                roles,
                options,
                pager);
        }

        async Task<IPagedResults<Role>> GetRoles(
            RoleIndexOptions options,
            PagerOptions pager)
        {
            return await _platoRoleStore.QueryAsync()
                .Take(pager.Page, pager.PageSize)
                .Select<RoleQueryParams>(q =>
                {
                    if (options.RoleId > 0)
                    {
                        q.Id.Equals(options.RoleId);
                    }
                    if (!string.IsNullOrEmpty(options.Search))
                    {
                        q.Keywords.Like(options.Search);
                    }
                })
                .OrderBy("ModifiedDate", OrderBy.Desc)
                .ToList();
        }

        async Task<IEnumerable<string>> GetEnabledRolePermissionsAsync(Role role)
        {

            // We can only obtain enabled permissions for existing roles
            // Return an empty list for new roles to avoid additional null checks
            if (role.Id == 0)
            {
                return new List<string>();
            }

            // If the role is anonymous set the auth type to
            // null to ensure IsAuthenticated is set to false
            var authType = role.Name != DefaultRoles.Anonymous
                ? "UserAuthType"
                : null;

            // Dummy identity
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, role.Name)
            }, authType);

            // Dummy principal
            var principal = new ClaimsPrincipal(identity);

            // Permissions grouped by feature
            var categorizedPermissions = await _permissionsManager.GetCategorizedPermissionsAsync();

            // Get flat permissions list from categorized permissions
            var permissions = categorizedPermissions.SelectMany(x => x.Value);

            var result = new List<string>();
            foreach (var permission in permissions)
            {
                if (await _authorizationService.AuthorizeAsync(principal, permission))
                {
                    result.Add(permission.Name);
                }
            }

            return result;

        }
        
        #endregion

    }
}
