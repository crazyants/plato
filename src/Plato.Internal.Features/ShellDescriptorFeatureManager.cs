using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Features;
using Plato.Internal.Models.Shell;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Shell.Abstractions;
using Plato.Internal.Stores.Abstractions.Shell;

namespace Plato.Internal.Features
{
    
    public class ShellDescriptorFeatureManager : IShellDescriptorFeatureManager
    {

        #region "Private Variables"

        // Build described features
        private ConcurrentDictionary<string, IShellFeature> _features;

        private readonly ConcurrentDictionary<string, IEnumerable<IShellFeature>> _featureDependencies
            = new ConcurrentDictionary<string, IEnumerable<IShellFeature>>();

        private readonly ConcurrentDictionary<string, IEnumerable<IShellFeature>> _dependentFeatures
            = new ConcurrentDictionary<string, IEnumerable<IShellFeature>>();

        #endregion

        #region "Constructor"

        private readonly IShellContextFactory _shellContextFactory;
        private readonly IModuleManager _moduleManager;
        private readonly IShellDescriptor _shellDescriptor;
        private readonly IShellDescriptorStore _shellDescriptorStore;

        public ShellDescriptorFeatureManager(
            IModuleManager moduleManager,
            IShellDescriptor shellDescriptor, 
            IShellDescriptorStore shellDescriptorStore,
            IShellContextFactory shellContextFactory)
        {
            _moduleManager = moduleManager;
            _shellDescriptor = shellDescriptor;
            _shellDescriptorStore = shellDescriptorStore;
            _shellContextFactory = shellContextFactory;
        }

        #endregion

        #region "Implementation"

        public async Task<IShellDescriptor> GetEnabledDescriptorAsync()
        {
            return await _shellDescriptorStore.GetAsync() ??
                _shellContextFactory.MinimumShellDescriptor();

        }

        public async Task<IEnumerable<IShellFeature>> GetEnabledFeaturesAsync()
        {

            // Get all enabled features
            var descriptor = await GetEnabledDescriptorAsync();
            
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
        
        public async Task<IEnumerable<IShellFeature>> GetFeaturesAsync()
        {

            // Load all features
            await EnsureFeaturesLoadedAsync();

            // Update dependencies
            await EnsureDependenciesAreEstablished();

            // Get explicitly enabled features
            var enabledFeatures = await GetEnabledFeaturesAsync();
            
            //  Update all found features to reflect enabled and required
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
        
        public async Task<IShellFeature> GetFeatureAsync(string featureId)
        {
            var features = await GetFeaturesAsync();
            return features.FirstOrDefault(f => f.Id == featureId);
        }

        public async Task<IEnumerable<IShellFeature>> GetFeaturesAsync(string[] featureIds)
        {
            var features = await GetFeaturesAsync();
            return features
                .Where(f => featureIds.Any(v => v.Equals(f.Id, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();
        }
        
        public async Task<IEnumerable<IShellFeature>> GetFeatureDependenciesAsync(string featureId)
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

        public async Task<IEnumerable<IShellFeature>> GetDepdendentFeaturesAsync(string featureId)
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
        
        /* General Idea...
             public static IEnumerable<T> Traverse<T>(T item, Func<T, IEnumerable<T>> childSelector)
            {
                var stack = new Stack<T>();
                stack.Push(item);
                while (stack.Any())
                {
                    var next = stack.Pop();
                    yield return next;
                    foreach (var child in childSelector(next))
                        stack.Push(child);
                }
            } */

        IEnumerable<IShellFeature> QueryDependencies(
            IShellFeature feature,
            IShellFeature[] features,
            Func<IShellFeature, IShellFeature[], IShellFeature[]> query)
        {

            var dependencies = new HashSet<IShellFeature>() { feature };

            var stack = new Stack<IShellFeature[]>();
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
                _features = new ConcurrentDictionary<string, IShellFeature>();
                var modules = await _moduleManager.LoadModulesAsync();
                if (modules != null)
                {
                    foreach (var module in modules)
                    {
                        var feature = new ShellFeature(module);
                        feature.IsRequired = IsFeatureRequired(feature);
                        _features.TryAdd(module.Descriptor.Id, feature);
                    }
                }
            }
        }
        
        bool IsFeatureRequired(ShellFeature feature)
        {
            // Mark SetUp as required
            if (feature.Id.Equals("Plato.SetUp", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            // Returns true if the supplied feature exists within our minimal shell descriptor
            var minimalDescriptor = _shellContextFactory.MinimumShellDescriptor();
            return minimalDescriptor.Modules.FirstOrDefault(m => m.Id.Equals(feature.Id, StringComparison.OrdinalIgnoreCase)) != null;
        }

        #endregion

    }
}
