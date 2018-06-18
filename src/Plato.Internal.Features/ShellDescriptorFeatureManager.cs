using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Models.Features;
using Plato.Internal.Models.Modules;
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


        // Build described features
        private ConcurrentDictionary<string, ShellFeature> _allFeatures;
        private IEnumerable<IModuleEntry> _modules;

        public async Task<IEnumerable<ShellFeature>> GetFeaturesAsync()
        {

            // Load all available modules
            await EnsureAvailableModulesAsync();

            // Update feature dependencies
            foreach (var feature in _allFeatures)
            {
                var newFeature = feature.Value;
                newFeature.FeatureDependencies = await GetFeatureDependencies(newFeature.Id);

                _allFeatures.TryUpdate(newFeature.Id, newFeature, feature.Value);
            }

            // Get explicitly enabled features and update dictionary to reflect enabled
            var enabledFeatures = await GetEnabledFeaturesAsync();
            foreach (var feature in enabledFeatures)
            {
                _allFeatures.AddOrUpdate(feature.Id, feature, (k, v) =>
                {
                    v.IsEnabled = true;
                    return v;
                });
            }
            
            return _allFeatures.Values;

        }
 

        async Task<IList<ShellFeature>> GetFeatureDependencies(string featureId)
        {

            var dependencies = new ConcurrentDictionary<string, ShellFeature>();
            RecurseDependencies(featureId);

            void RecurseDependencies(string id)
            {
                // Get feature module
                var module = _modules.FirstOrDefault(m => m.Descriptor.Id == id);

                // Ensure we have dependencies
                if (module != null && module.Descriptor.Dependencies.Any())
                {
                    // Recurse all dependencids
                    foreach (var dependency in module.Descriptor.Dependencies)
                    {
                        var notInList = !dependencies.ContainsKey(dependency.Id);
                        var notCurrent = dependency.Id != featureId;
                        if (notInList && notCurrent)
                        {
                            dependencies.TryAdd(dependency.Id, new ShellFeature(dependency.Id, dependency.Version));
                            RecurseDependencies(dependency.Id);
                        }
                     
                    }
                }
            }

            // Ensure distinct ordered results
            return dependencies.Values.Distinct().ToList();

        }


        async Task EnsureAvailableModulesAsync()
        {

            // Ensure we only load modules once 
            if (_allFeatures == null)
            {
                // Get all available modules and convert to features
                _allFeatures = new ConcurrentDictionary<string, ShellFeature>();
                _modules = await _moduleManager.LoadModulesAsync();
                foreach (var module in _modules)
                {
                    _allFeatures.AddOrUpdate(module.Descriptor.Id, new ShellFeature(module), (k, v) => v);
                }
            }

        }
    }
}
