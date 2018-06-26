using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Internal.Models.Features;
using Plato.Internal.Models.Shell;
using Plato.Internal.Stores.Abstract;
using Plato.Internal.Stores.Abstractions.Shell;

namespace Plato.Internal.Stores.Shell
{
    public class ShellDescriptorStore : IShellDescriptorStore
    {

        private readonly string _key = CacheKeys.ShellDescriptor.ToString();

        private readonly IDictionaryStore _dictionaryStore;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ShellDescriptorStore> _logger;
        private readonly IShellFeatureStore<ShellFeature> _shellFeatureStore;

        public ShellDescriptorStore(
            IShellFeatureStore<ShellFeature> shellFeatureStore,
            IDictionaryStore dictionaryStore,
            IMemoryCache memoryCache, 
            ILogger<ShellDescriptorStore> logger)
        {
            _dictionaryStore = dictionaryStore;
            _memoryCache = memoryCache;
            _logger = logger;
            _shellFeatureStore = shellFeatureStore;
        }

        #region "Implementation"

        public async Task<IShellDescriptor> GetAsync()
        {
            if (!_memoryCache.TryGetValue(_key, out IShellDescriptor descriptor))
            {
                descriptor = await _dictionaryStore.GetAsync<ShellDescriptor>(_key);
                if (descriptor != null)
                    _memoryCache.Set(_key, descriptor);
            }

            return descriptor;
        }

        public async Task<IShellDescriptor> SaveAsync(IShellDescriptor shellDescriptor)
        {
            
            // Add & get features
            var features = await AddAndGetFeaturesAsync(shellDescriptor.Modules);;
            shellDescriptor.Modules = features.ToList();
            
            // Update descriptor
            var descriptor = await _dictionaryStore.UpdateAsync<ShellDescriptor>(_key, shellDescriptor);
            if (descriptor != null)
            {
                _memoryCache.Set(_key, descriptor);
            }

            return descriptor;
        }

        public async Task<bool> DeleteAsync()
        {
            var result = await _dictionaryStore.DeleteAsync(_key);
            _memoryCache.Remove(_key);
            return result;
        }

        #endregion

        #region "Private Methods"

        private async Task<IEnumerable<ShellModule>> AddAndGetFeaturesAsync(IEnumerable<ShellModule> modulesToAdd)
        {

            // Ensure we have a distinct list of features to save
            var distinctDictionary = new ConcurrentDictionary<string, ShellModule>();
            foreach (var module in modulesToAdd)
            {
                if (!distinctDictionary.ContainsKey(module.ModuleId))
                {
                    distinctDictionary.TryAdd(module.ModuleId, module);
                }
            }

            // Distint list of features to add
            var featuresToAdd = distinctDictionary.Values.ToList();

            // Get all currently registered features
            var currentFeatures = await _shellFeatureStore.SelectFeatures();
            
            // Conver to list
            var currentFeaturesList = currentFeatures.ToList();

            // Delete any featur not in our current list of features to add
            foreach (var feature in currentFeaturesList)
            {
                if (featuresToAdd.FirstOrDefault(f => f.ModuleId == feature.ModuleId) == null)
                {
                    await _shellFeatureStore.DeleteAsync(feature);
                }
            }
            
            // Add any new features into our features table
            var output = new List<ShellModule>();
            foreach (var feature in featuresToAdd)
            {
                // add or update feature
                var existingFeature = currentFeaturesList.FirstOrDefault(f => f.ModuleId == feature.ModuleId)
                                      ?? new ShellFeature()
                                      {
                                          ModuleId = feature.ModuleId,
                                          Version = feature.Version
                                      };

                var newOrUpdatedFeature = await _shellFeatureStore.CreateAsync(existingFeature);
                if (newOrUpdatedFeature != null)
                {
                    output.Add(new ShellModule(newOrUpdatedFeature));
                }

            }
            
            return output;

        }

        #endregion


    }

}
