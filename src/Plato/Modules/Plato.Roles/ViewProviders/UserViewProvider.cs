using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Roles.ViewModels;

namespace Plato.Roles.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<User>
    {

        private const string HtmlName = "UserRoles";
        
        private readonly IPlatoRoleStore _platoRoleStore;
        private readonly UserManager<User> _userManager;
        private readonly HttpRequest _request;
        private readonly IStringLocalizer T;
        
        public UserViewProvider(
            IStringLocalizer<UserViewProvider> stringLocalizee,
            IHttpContextAccessor httpContextAccessor,
            IPlatoRoleStore platoRoleStore,
            UserManager<User> userManager)
        {
      
            _request = httpContextAccessor.HttpContext.Request;
            _platoRoleStore = platoRoleStore;
            _userManager = userManager;

            T = stringLocalizee;
    
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(User user, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(User user, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(User user, IViewProviderContext updater)
        {

            IEnumerable<string> selectedRoles = null;
            if (user.Id > 0)
            {
                selectedRoles = await _platoRoleStore.GetRoleNamesByUserIdAsync(user.Id);
            }

            // When adding new users ensure the Member role is selected by default
            // When editing an existing user with no roles use an empty list
            var defaultRoles = user.Id == 0
                ? new List<string>()
                {
                    DefaultRoles.Member
                }
                : new List<string>();

            return Views(
                View<EditUserRolesViewModel>("User.Roles.Edit.Content", model =>
                {
                    model.SelectedRoles = selectedRoles ?? defaultRoles;
                    model.HtmlName = HtmlName;
                    return model;
                }).Order(int.MaxValue - 100)
            );

        }

        public override async Task ComposeModelAsync(User user, IUpdateModel updater)
        {

            // Build selected roles
            var rolesToAdd = GetSelectedRoles();

            // Update model
            var model = new EditUserRolesViewModel
            {
                SelectedRoles = rolesToAdd
            };
            
            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {
                user.RoleNames = model.SelectedRoles;
            }

        }


        public override async Task<IViewProviderResult> BuildUpdateAsync(User user, IViewProviderContext context)
        {
   
            // Build selected roles
            var rolesToAdd = GetSelectedRoles();

            // Update model
            var model = new EditUserRolesViewModel
            {
                SelectedRoles = rolesToAdd
            };

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(user, context);
            }

            if (context.Updater.ModelState.IsValid)
            {

                var rolesToRemove = new List<string>();
                foreach (var role in await _userManager.GetRolesAsync(user))
                {
                    if (!rolesToAdd.Contains(role))
                    {
                        rolesToRemove.Add(role);
                    }
                }

                foreach (var role in rolesToRemove)
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }

                // Add new roles
                foreach (var role in rolesToAdd)
                {
                    if (!await _userManager.IsInRoleAsync(user, role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }
                }

                // Update user
                var result = await _userManager.UpdateAsync(user);
                foreach (var error in result.Errors)
                {
                    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }
           
            return await BuildEditAsync(user, context);

        }

        private IList<string> GetSelectedRoles()
        {
            var output = new List<string>();
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(HtmlName))
                {
                    var values = _request.Form[key];
                    foreach (var value in values)
                    {
                        if (!output.Contains(value))
                        {
                            output.Add(value);
                        }

                    }
                }
            }

            return output;

        }

    }

}
