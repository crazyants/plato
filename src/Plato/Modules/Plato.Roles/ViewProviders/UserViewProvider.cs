using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Roles.ViewModels;

namespace Plato.Roles.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<User>
    {

        private const string HtmlName = "UserRoles";

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IPlatoRoleStore _platoRoleStore;
        private readonly IContextFacade _contextFacade;
        private readonly HttpRequest _request;

        private readonly IStringLocalizer T;
        

        public UserViewProvider(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IPlatoRoleStore platoRoleStore,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<UserViewProvider> stringLocalize, IContextFacade contextFacade)
        {
            _userManager = userManager;
            _platoRoleStore = platoRoleStore;
            _request = httpContextAccessor.HttpContext.Request;

            T = stringLocalize;
            _contextFacade = contextFacade;
            _signInManager = signInManager;
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

            var selectedRoles = await _platoRoleStore.GetRoleNamesByUserIdAsync(user.Id);

            return Views(
                View<EditUserRolesViewModel>("User.Roles.Edit.Content", model =>
                    {
                        model.SelectedRoles = selectedRoles;
                        model.HtmlName = HtmlName;
                        return model;
                    }).Order(2)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(User user, IViewProviderContext context)
        {

            // Get available role names
            var roleNames = await _platoRoleStore.GetRoleNamesAsync();

            // Build selected roles
            var rolesToAdd = new List<string>();
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(HtmlName))
                {
                    var values = _request.Form[key];
                    foreach (var value in values)
                    {
                        if (!rolesToAdd.Contains(value))
                        {
                            rolesToAdd.Add(value);
                        }
                        
                    }
                }
            }
            
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
                var hasRoleChanges = false;
                var rolesToRemove = new List<string>();
                foreach (var role in await _userManager.GetRolesAsync(user))
                {
                    if (!rolesToAdd.Contains(role))
                    {
                        hasRoleChanges = true;
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
                        hasRoleChanges = true;
                        await _userManager.AddToRoleAsync(user, role);
                    }
                }

                // Update user
                var result = await _userManager.UpdateAsync(user);
                foreach (var error in result.Errors)
                {
                    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                }

                // Re-signin current authenticated User to reflect
                // the claim changes if roles are updated within the identity cookie
                if (hasRoleChanges)
                {

                    var currentUser = await _contextFacade.GetAuthenticatedUserAsync();
                    if (currentUser == null)
                    {
                        return await BuildEditAsync(user, context);
                    }

                    if (currentUser.Id == user.Id)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                    }
              
                }

            }
           
            return await BuildEditAsync(user, context);

        }

    }
}
