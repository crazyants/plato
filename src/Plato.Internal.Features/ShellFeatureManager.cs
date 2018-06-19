using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Internal.Models.Features;
using Plato.Internal.Models.Shell;
using Plato.Internal.Stores.Abstractions.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Internal.Features
{

    public class ShellFeatureManager : IShellFeatureManager
    {

        private readonly IPlatoHost _platoHost;
        private readonly IShellDescriptorStore _shellDescriptorStore;
        private readonly IShellDescriptorFeatureManager _shellDescriptorFeatureManager;
        private readonly IServiceCollection _applicationServices;
        private readonly IRunningShellTable _runningShellTable;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFeatureEventManager _featureEventManager;
        private readonly ILogger<ShellFeatureManager> _logger;
        
        public ShellFeatureManager(
            IShellDescriptorStore shellDescriptorStore,
            IShellDescriptorFeatureManager shellDescriptorFeatureManager,
            ILogger<ShellFeatureManager> logger,
            IServiceCollection applicationServices,
            IRunningShellTable runningShellTable, 
            IHttpContextAccessor httpContextAccessor,
            IFeatureEventManager featureEventManager,
            IPlatoHost platoHost)
        {
            _shellDescriptorStore = shellDescriptorStore;
            _shellDescriptorFeatureManager = shellDescriptorFeatureManager;
            _logger = logger;
            _applicationServices = applicationServices;
            _runningShellTable = runningShellTable;
            _httpContextAccessor = httpContextAccessor;
            _featureEventManager = featureEventManager;
            _platoHost = platoHost;
        }

        #region "Implementation"
        
        public async Task<IEnumerable<IFeatureEventContext>> EnableFeatureAsync(string featureId)
        {

            // Get features to enable
            var features = await _shellDescriptorFeatureManager.GetFeatureAsync(featureId);

            // Ensure we also enable dependencies
            var featureIds = features.FeatureDependencies
                .Select(d => d.Id).ToArray();
            
            // Enable features
            return await EnableFeaturesAsync(featureIds);

        }
        
        public async Task<IEnumerable<IFeatureEventContext>> EnableFeaturesAsync(string[] featureIds)
        {

            // Get distinct Ids
            var ids = featureIds.Distinct().ToArray();
            
            // Get features to enable
            var features = await _shellDescriptorFeatureManager.GetFeaturesAsync(ids);
            
            // Conver to IList to work with
            var featureList = features.ToList();
            
            // Holds the results of all our event executation contexts
            var contexts = new ConcurrentDictionary<string, IFeatureEventContext>();
            
            // Raise installing events for features
            InvokeFeaturesRecursivly(featureList,
                async feature =>
                {
                    // Ensure feature is not already enabled
                    if (!feature.IsEnabled)
                    {
                        contexts.TryAdd(feature.Id,
                            await _featureEventManager.InstallingAsync(new FeatureEventContext(feature)));
                    }
                    
                });

            // Did any event encounter errors?
            var hasErrors = contexts.Where(c => c.Value.Errors.Count > 0);

            // No errors update descriptor, raise InstalledAsync and recycle ShellContext
            if (!hasErrors.Any())
            {

                // Update descriptor within database
                var descriptor = await GetOrUpdateDescriptor(featureIds);
                var updatedDescriptor = await _shellDescriptorStore.SaveAsync(descriptor);

                // Raise Installed event
                InvokeFeaturesRecursivly(featureList,
                    async feature =>
                    {  // Ensure feature is not already enabled
                        if (!feature.IsEnabled)
                        {
                            var context = await _featureEventManager.InstalledAsync(new FeatureEventContext(feature));
                            contexts.TryUpdate(feature.Id, context, context);
                        }
                    });

                // dispose current shell context
                RecycleShell();

            }

            // Return all execution contexts
            return contexts.Values;

        }
        
        public async Task<IEnumerable<IFeatureEventContext>> DisableFeatureAsync(string featureId)
        {

            // Get features to enable
            var features = await _shellDescriptorFeatureManager.GetFeatureAsync(featureId);

            // Ensure we also disable dependent features
            var featureIds = features.DependentFeatures
                .Select(d => d.Id).ToArray();

            return await DisableFeaturesAsync(featureIds);

        }
        
        public async Task<IEnumerable<IFeatureEventContext>> DisableFeaturesAsync(string[] featureIds)
        {

            // Get distinct Ids
            var ids = featureIds.Distinct().ToArray();

            // Get features to enable
            var features = await _shellDescriptorFeatureManager.GetFeaturesAsync(ids);

            // Conver to IList to work with
            var featureList = features.ToList();

            // Holds the results of all our event executation contexts
            var contexts = new ConcurrentDictionary<string, IFeatureEventContext>();

            // Raise Uninstalling events
            InvokeFeaturesRecursivly(featureList,
                async feature =>
                {
                    // Ensure feature is enabled
                    if (feature.IsEnabled)
                    {
                        contexts.TryAdd(feature.Id,
                            await _featureEventManager.UninstallingAsync(new FeatureEventContext(feature)));
                    }
                });

            // Did any event encounter errors?
            var hasErrors = contexts.Where(c => c.Value.Errors.Count > 0);

            // No errors update descriptor, raise InstalledAsync and recycle ShellContext
            if (!hasErrors.Any())
            {

                // Update features within data store
                var descriptor = await RemoveFeaturesFromCurrentDescriptor(featureIds);
                var updatedDescriptor = await _shellDescriptorStore.SaveAsync(descriptor);

                // Raise Uninstalled events for features
                InvokeFeaturesRecursivly(featureList,
                    async feature =>
                    {
                        // Ensure feature is enabled
                        if (feature.IsEnabled)
                        {
                            var context = await _featureEventManager.UninstallingAsync(new FeatureEventContext(feature));
                            contexts.TryUpdate(feature.Id, context, context);
                            
                        }
                    });

                // Dispose current shell context
                RecycleShell();

            }
            
            // Return all execution contexts
            return contexts.Values;

        }

        #endregion

        #region "Private Methods"

        async Task<IShellDescriptor> RemoveFeaturesFromCurrentDescriptor(string[] featureIds)
        {
            // First get all existing enabled features
            var enabledFeatures = await _shellDescriptorFeatureManager.GetEnabledFeaturesAsync();

            // Add features minus our features to disable
            var descriptor = new ShellDescriptor();
            foreach (var feature in enabledFeatures)
            {
                var diable = featureIds.Any(f => f.Equals(feature.Id, StringComparison.InvariantCultureIgnoreCase));
                if (!diable)
                {
                    descriptor.Modules.Add(new ShellModule(feature.Id));
                }
            }

            return descriptor;

        }

        async Task<IShellDescriptor> GetOrUpdateDescriptor(string[] featureIds)
        {

            // Get existing descriptor or create a new one
            var descriptor =
                await _shellDescriptorStore.GetAsync()
                ?? new ShellDescriptor();

            // Add features to our descriptor
            foreach (var featureId in featureIds)
            {
                descriptor.Modules.Add(new ShellModule(featureId));
            }

            return descriptor;
        }

        void InvokeFeaturesRecursivly(
            IEnumerable<IShellFeature> features,
            Action<IShellFeature> invoker)
        {
            foreach (var feature in features)
            {
                invoker(feature);
            }
        }

        void RecycleShell()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var shellSettings = _runningShellTable.Match(httpContext);
            _platoHost.RecycleShellContext(shellSettings);
        }

        #endregion
        

    }
}
