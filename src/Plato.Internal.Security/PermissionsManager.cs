using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Plato.Internal.Models.Roles;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Internal.Security
{
    
    public class PermissionsManager : IPermissionsManager
    {

        private readonly IAuthorizationService _authorizationService;
        private readonly IEnumerable<IPermissionsProvider> _providers;
        private readonly ILogger<PermissionsManager> _logger;
        private readonly ITypedModuleProvider _typedModuleProvider;

        public PermissionsManager(
            IEnumerable<IPermissionsProvider> providers,
            ILogger<PermissionsManager> logger,
            ITypedModuleProvider typedModuleProvider,
            IAuthorizationService authorizationService)
        {
            _providers = providers;
            _typedModuleProvider = typedModuleProvider;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        private IEnumerable<Permission> _permissions;

        public IEnumerable<Permission> GetPermissions()
        {

            // Ensure permissions are only loaded once per scope
            if (_permissions == null)
            {
                var permissions = new List<Permission>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        permissions.AddRange(provider.GetPermissions());
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,
                            $"An exception occurred within the permissions provider. Please review your permission provider and try again. {e.Message}");
                        throw;
                    }
                }

                _permissions = permissions;
            }

            return _permissions;

        }

        public async Task<IDictionary<string, IEnumerable<Permission>>> GetCategorizedPermissionsAsync()
        {

            var output = new Dictionary<string, IEnumerable<Permission>>();

            foreach (var provider in _providers)
            {

                var module = await _typedModuleProvider.GetModuleForDependency(provider.GetType());
                var name = module.Descriptor.Name;
                var permissions = provider.GetPermissions();
                foreach (var permission in permissions)
                {
                    var category = permission.Category;
                    var title = String.IsNullOrWhiteSpace(category) ? 
                        name :
                        category;

                    if (output.ContainsKey(title))
                    {
                        output[title] = output[title].Concat(new[] { permission });
                    }
                    else
                    {
                        output.Add(title, new[] { permission });
                    }
                }
            }

            return output;
        }

        public async Task<IEnumerable<string>> GetEnabledRolePermissionsAsync(Role role)
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
            var categorizedPermissions = await GetCategorizedPermissionsAsync();

            // Get flat permissions list from categorized permissions
            var permissions = categorizedPermissions.SelectMany(x => x.Value);

            var result = new List<string>();
            foreach (var permission in permissions)
            {
                if (await _authorizationService.AuthorizeAsync(principal,  permission))
                {
                    result.Add(permission.Name);
                }
            }

            return result;

        }
    }
    


}
