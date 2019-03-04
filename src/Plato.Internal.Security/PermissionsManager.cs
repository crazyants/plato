using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Internal.Security
{
    
    public class PermissionsManager<TPermission> : IPermissionsManager<TPermission> where TPermission : class, IPermission
    {

        private IEnumerable<TPermission> _permissions;
        
        private readonly IAuthorizationService _authorizationService;
        private readonly IEnumerable<IPermissionsProvider<TPermission>> _providers;
        private readonly ILogger<PermissionsManager<TPermission>> _logger;
        private readonly ITypedModuleProvider _typedModuleProvider;

        public PermissionsManager(
            IEnumerable<IPermissionsProvider<TPermission>> providers,
            ILogger<PermissionsManager<TPermission>> logger,
            ITypedModuleProvider typedModuleProvider,
            IAuthorizationService authorizationService)
        {
            _providers = providers;
            _typedModuleProvider = typedModuleProvider;
            _authorizationService = authorizationService;
            _logger = logger;
        }
        
        public IEnumerable<TPermission> GetPermissions()
        {
            
            // Ensure we only load permissions once
            if (_permissions == null)
            {
                var permissions = new List<TPermission>();
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

        public async Task<IDictionary<string, IEnumerable<TPermission>>> GetCategorizedPermissionsAsync()
        {

            var output = new Dictionary<string, IEnumerable<TPermission>>();

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

    }
    
}
