using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Models.Features;
using Plato.Internal.Models.Shell;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Stores.Abstractions.Shell;

namespace Plato.Internal.Features
{

    public interface IShellDescriptorFeatureManager
    {

        Task<IEnumerable<ShellFeature>> GetEnabledFeaturesAsync();

        Task<IEnumerable<ShellFeature>> GetFeaturesAsync();

    }

    public class ShellDescriptorFeatureManager : IShellDescriptorFeatureManager
    {

        private readonly IModuleManager _moduleManager;
        private readonly IShellDescriptor _shellDescriptor;
        private readonly IShellDescriptorStore _shellDescriptorStore;

        public ShellDescriptorFeatureManager(
            IModuleManager moduleManager,
            IShellDescriptor shellDescriptor, 
            IShellDescriptorStore shellDescriptorStore)
        {
            _moduleManager = moduleManager;
            _shellDescriptor = shellDescriptor;
            _shellDescriptorStore = shellDescriptorStore;
        }
        
        
        public async Task<IEnumerable<ShellFeature>> GetEnabledFeaturesAsync()
        {

            // Get all features enabled within the database
            var descriptor = await _shellDescriptorStore.GetAsync();
            var features = new List<ShellFeature>();
            if (descriptor != null)
            {
                foreach (var module in descriptor.Modules)
                {
                    features.Add(new ShellFeature(module.Id));
                }
            }
            return features;

        }

        public async Task<IEnumerable<ShellFeature>> GetFeaturesAsync()
        {

            // Build described features
            var describedFeatures = new ConcurrentDictionary<string, ShellFeature>();
            
            // Get all availablke modules and convert to features
            var modules = await _moduleManager.LoadModulesAsync();
            foreach (var module in modules)
            {
                describedFeatures.AddOrUpdate(module.Descriptor.Id, new ShellFeature(module), (k, v) => v);
            }
            
          // Get explicitly enabled features and update dictionary to reflect enabled
            var enabledFeatures = await GetEnabledFeaturesAsync();
            foreach (var feature in enabledFeatures)
            {
                describedFeatures.AddOrUpdate(feature.Id, feature, (k, v) =>
                {
                    v.IsEnabled = true;
                    return v;
                });
            }
            
            return describedFeatures.Values;

        }
        
    }
}
