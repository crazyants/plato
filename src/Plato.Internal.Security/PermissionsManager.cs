using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Internal.Security
{
    
    public class PermissionsManager : IPermissionsManager
    {

        private readonly IEnumerable<IPermissionsProvider> _providers;
        private readonly ILogger<PermissionsManager> _logger;
        private readonly ITypedModuleProvider _typedModuleProvider;

        public PermissionsManager(
            IEnumerable<IPermissionsProvider> providers,
            ILogger<PermissionsManager> logger,
            ITypedModuleProvider typedModuleProvider)
        {
            _providers = providers;
            _logger = logger;
            _typedModuleProvider = typedModuleProvider;
        }
        
        public IEnumerable<Permission> GetPermissions()
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
                    _logger.LogError(e, $"An exception occurred within the permissions provider. Please review your permission provider and try again. {e.Message}");
                    throw;
                }
            }

            return permissions;

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
        

    }

}
