using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Plato.Discuss.Moderation.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Roles;
using Plato.Internal.Models.Shell;
using Plato.Internal.Models.Users;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Moderation.Models;
using Plato.Moderation.Stores;

namespace Plato.Discuss.Moderation.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<Moderator>
    {

        private readonly IContextFacade _contextFacade;
        private readonly IPermissionsManager2<ModeratorPermission> _permissionsManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IModeratorStore<Moderator> _moderatorStore;
        private readonly IPlatoUserStore<User> _userStore;
        private readonly HttpRequest _request;

        public AdminViewProvider(
            IContextFacade contextFacade,
            IPermissionsManager2<ModeratorPermission> permissionsManager,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IModeratorStore<Moderator> moderatorStore, 
            IPlatoUserStore<User> userStore)
        {
            _contextFacade = contextFacade;
            _permissionsManager = permissionsManager;
            _authorizationService = authorizationService;
            _moderatorStore = moderatorStore;
            _userStore = userStore;
            _request = httpContextAccessor.HttpContext.Request;
        }

        #region "Implementation"

        public override async Task<IViewProviderResult> BuildIndexAsync(Moderator moderator, IUpdateModel updater)
        {
            var viewModel = await GetIndexModel();

            return Views(
                View<ModeratorIndexViewModel>("Admin.Index.Header", model => viewModel).Zone("header").Order(1),
                View<ModeratorIndexViewModel>("Admin.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<ModeratorIndexViewModel>("Admin.Index.Content", model => viewModel).Zone("content").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Moderator oldModerator, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));

        }

        public override async Task<IViewProviderResult> BuildEditAsync(Moderator oldModerator, IUpdateModel updater)
        {

            var viewModel = new EditModeratorViewModel
            {
                IsNewModerator = oldModerator.UserId == 0,
                EnabledPermissions = await GetEnabledRolePermissionsAsync(new Role()),
                CategorizedPermissions = await _permissionsManager.GetCategorizedPermissionsAsync()
            };

            return Views(
                View<EditModeratorViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<EditModeratorViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1),
                View<EditModeratorViewModel>("Admin.Edit.Actions", model => viewModel).Zone("actions").Order(1),
                View<EditModeratorViewModel>("Admin.Edit.Footer", model => viewModel).Zone("footer").Order(1)
            );
        }


        public override async Task<bool> ValidateModelAsync(Moderator moderator, IUpdateModel updater)
        {
            var valid = await updater.TryUpdateModelAsync(new EditModeratorViewModel()
            {
                UserId = moderator.UserId
            });

            return valid;
        }

        public override async Task ComposeTypeAsync(Moderator moderator, IUpdateModel updater)
        {

            var model = new EditModeratorViewModel
            {
                UserId = moderator.UserId
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {
                moderator.UserId = model.UserId;
            }

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Moderator model, IUpdateModel updater)
        {


            var moderator = await _moderatorStore.GetByIdAsync(model.Id);
            if (moderator == null)
            {
                return await BuildIndexAsync(model, updater);
            }
          
            // Validate model
            if (await ValidateModelAsync(model, updater))
            {

                // Build a list of claims to add or update
                var moderatorClaims = new List<ModeratorClaim>();
                foreach (var key in _request.Form.Keys)
                {
                    if (key.StartsWith("Checkbox.") && _request.Form[key] == "true")
                    {
                        var permissionName = key.Substring("Checkbox.".Length);
                        moderatorClaims.Add(new ModeratorClaim { ClaimType = ModeratorPermission.ClaimType, ClaimValue = permissionName });
                    }
                }
                
                // Update claims
                moderator.Claims = moderatorClaims;
                
                // Persist moderator
                await _moderatorStore.UpdateAsync(moderator);

            }

            return await BuildEditAsync(moderator, updater);


        }

        #endregion

        #region "Private Methods"

   

        async Task<ModeratorIndexViewModel> GetIndexModel()
        {
            var feature = await GetcurrentFeature();
            
            return new ModeratorIndexViewModel()
            {
             
            };
        }

        async Task<IEnumerable<string>> GetEnabledRolePermissionsAsync(Role role)
        {

            // We can only obtain enabled permissions for existing roles
            // Return an empty list for new roles to avoid additional null checks
            if (role.Id == 0)
            {
                return new List<string>();
            }

            // If the role is anonymous set the authtype to
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


        async Task<ShellModule> GetcurrentFeature()
        {
            var featureId = "Plato.Discuss.Labels";
            var feature = await _contextFacade.GetFeatureByModuleIdAsync(featureId);
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the module '{featureId}'");
            }
            return feature;
        }

        #endregion

    }
    
    public class TagItItem
    {

        public string Text { get; set; }

        public string Value { get; set; }
    }


}
