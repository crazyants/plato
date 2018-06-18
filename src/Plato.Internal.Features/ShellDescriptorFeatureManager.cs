using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        Task<IEnumerable<ShellFeature>> GetFeatureDependenciesAsync(string featureId);

        Task<IEnumerable<ShellFeature>> GetDepdendentFeaturesAsync(string featureId);

    }

    public class ShellDescriptorFeatureManager : IShellDescriptorFeatureManager
    {


        // Build described features
        private ConcurrentDictionary<string, ShellFeature> _features;

        private readonly ConcurrentDictionary<string, IEnumerable<ShellFeature>> _featureDependencies
            = new ConcurrentDictionary<string, IEnumerable<ShellFeature>>();

        private readonly ConcurrentDictionary<string, IEnumerable<ShellFeature>> _dependentFeatures
            = new ConcurrentDictionary<string, IEnumerable<ShellFeature>>();
        
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

        #region "Implementation"

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

            // Load all features
            await EnsureFeaturesLoadedAsync();

            // Update dependencies
            await EnsureDependenciesAreEstablished();

            // Get explicitly enabled features and update features to reflect enabled
            var enabledFeatures = await GetEnabledFeaturesAsync();
            foreach (var feature in enabledFeatures)
            {
                _features.AddOrUpdate(feature.Id, feature, (k, v) =>
                {
                    v.IsEnabled = true;
                    return v;
                });
            }
            
            return _features.Values;

        }


        public async Task<IEnumerable<ShellFeature>> GetFeatureDependenciesAsync(string featureId)
        {
            // Load minimal features
            await EnsureFeaturesLoadedAsync();
            return _featureDependencies.GetOrAdd(featureId, key =>
            {

                if (!_features.ContainsKey(key))
                {
                    return Enumerable.Empty<ShellFeature>();
                }

                var feature = _features[key];
                return QueryDependencies(
                    feature,
                    _features.Values.ToArray(),
                    (currentFeature, fs) => fs
                        .Where(f => currentFeature.Dependencies.Any(dep => dep.Id == f.Id))
                        .ToArray());

            });
        }

        public async Task<IEnumerable<ShellFeature>> GetDepdendentFeaturesAsync(string featureId)
        {
            // Load minimal features
            await EnsureFeaturesLoadedAsync();
            return _dependentFeatures.GetOrAdd(featureId, key =>
            {

                if (!_features.ContainsKey(key))
                {
                    return Enumerable.Empty<ShellFeature>();
                }

                var feature = _features[key];
                return QueryDependencies(
                    feature,
                    _features.Values.ToArray(),
                    (currentFeature, fs) => fs
                        .Where(f => f.Dependencies.Any(dep => dep.Id == currentFeature.Id))
                        .ToArray());

            });
        }

        #endregion

        #region "Private Methods"

        async Task EnsureDependenciesAreEstablished()
        {

            foreach (var feature in _features.Values)
            {
                var f = feature;
                f.FeatureDependencies = await GetFeatureDependenciesAsync(f.Id);
                f.DependentFeatures = await GetDepdendentFeaturesAsync(f.Id);
                _features.TryUpdate(f.Id, f, feature);
            }

        }

        IEnumerable<ShellFeature> QueryDependencies(
            ShellFeature feature,
            ShellFeature[] features,
            Func<ShellFeature, ShellFeature[], ShellFeature[]> query)
        {

            var dependencies = new HashSet<ShellFeature>() { feature };

            var stack = new Stack<ShellFeature[]>();
            stack.Push(query(feature, features));

            while (stack.Count > 0)
            {
                var next = stack.Pop();
                foreach (var dependency in next.Where(dependency => !dependencies.Contains(dependency)))
                {
                    dependencies.Add(dependency);
                    stack.Push(query(dependency, features));
                }
            }

            return _features
                .Where(f => dependencies.Any(d => d.Id == f.Value.Id))
                .Select(f => f.Value);

        }
        
        async Task EnsureFeaturesLoadedAsync()
        {
            if (_features == null)
            {
                _features = new ConcurrentDictionary<string, ShellFeature>();
                var modules = await _moduleManager.LoadModulesAsync();
                if (modules != null)
                {
                    foreach (var module in modules)
                    {
                        _features.TryAdd(module.Descriptor.Id, new ShellFeature(module));
                    }
                }
            }
        }

        #endregion

    }
}
