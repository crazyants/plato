using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Roles;
using Plato.Internal.Models.Users;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Roles.ViewModels;

namespace Plato.Roles.ViewProviders
{
    public class RoleViewProvider : BaseViewProvider<Role>
    {

        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IPlatoRoleStore _platoRoleStore;
        private readonly IPermissionsManager _permissionsManager;

        public RoleViewProvider(
            UserManager<User> userManager,
            IPlatoRoleStore platoRoleStore,
            RoleManager<Role> roleManager, 
            IPermissionsManager permissionsManager)
        {
            _userManager = userManager;
            _platoRoleStore = platoRoleStore;
            _roleManager = roleManager;
            _permissionsManager = permissionsManager;
        
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Role role, IUpdateModel updater)
        {

            return Task.FromResult(
                Views(
                    View<Role>("Admin.Display.Header", model => role).Zone("header"),
                    View<Role>("Admin.Display.Meta", model => role).Zone("meta"),
                    View<Role>("Admin.Display.Content", model => role).Zone("content"),
                    View<Role>("Admin.Display.Footer", model => role).Zone("footer")
                ));

        }

        public override Task<IViewProviderResult> BuildIndexAsync(Role role, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(Role role, IUpdateModel updater)
        {
        
            var editRoleViewModel = new EditRoleViewModel()
            {
                Id = role.Id,
                RoleName = role.Name,
                Role = role,
                IsNewRole = await IsNewRole(role.Id),
                EnabledPermissions = await _permissionsManager.GetEnabledRolePermissionsAsync(role),
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

        public override async Task<IViewProviderResult> BuildUpdateAsync(Role role, IUpdateModel updater)
        {

            var model = new EditRoleViewModel();

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(role, updater);
            }

            if (updater.ModelState.IsValid)
            {

                role.Name = model.RoleName?.Trim();
                
                var result = await _roleManager.CreateAsync(role);
                foreach (var error in result.Errors)
                {
                    updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return await BuildEditAsync(role, updater);

        }

        private async Task<bool> IsNewRole(int roleId)
        {
            return await _roleManager.FindByIdAsync(roleId.ToString()) == null;
        }

    }
}
