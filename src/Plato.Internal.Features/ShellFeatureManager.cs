using System;
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

        public async Task<IEnumerable<IShellFeature>> EnableFeaturesAsync(string[] featureIds)
        {

            // Get features to enable
            var features = await _shellDescriptorFeatureManager.GetFeaturesAsync(featureIds);
            
            // Conver to IList to work with
            var featureList = features.ToList();

            // Raise installing events for features
            await InvokeFeaturesRecursivly(featureList, async feature =>
            {
                await _featureEventManager.InstallingAsync(feature);
            });
            
            // Update descriptor within database
            var descriptor = await GetOrUpdateDescriptor(featureIds);
            var updatedDescriptor = await _shellDescriptorStore.SaveAsync(descriptor);

            // Raise installed event for feature
            await InvokeFeaturesRecursivly(featureList, async feature =>
            {
                await _featureEventManager.InstalledAsync(feature);
            });
       
            // dispose current shell context
            RecycleShell();

            return null;

        }
        
        public async Task<IEnumerable<IShellFeature>> DisableFeaturesAsync(string[] featureIds)
        {
            
            // Get features to enable
            var features = await _shellDescriptorFeatureManager.GetFeaturesAsync(featureIds);

            // Conver to IList to work with
            var featureList = features.ToList();

            // Raise Uninstalling events for features
            await InvokeFeaturesRecursivly(featureList, async feature =>
            {
                await _featureEventManager.UninstallingAsync(feature);
            });
            
            // Update features within data store
            var descriptor = await RemoveFeaturesFromCurrentDescriptor(featureIds);
            var updatedDescriptor = await _shellDescriptorStore.SaveAsync(descriptor);

            // Raise Uninstalled events for features
            await InvokeFeaturesRecursivly(featureList, async feature =>
            {
                await _featureEventManager.UninstallingAsync(feature);
            });
            
            // dispose current shell context
            RecycleShell();

            return null;

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

        async Task InvokeFeaturesRecursivly(
            IEnumerable<IShellFeature> features,
            Action<IShellFeature> invoker)
        {

            foreach (var feature in features)
            {
                invoker(feature);
                if (feature.FeatureDependencies.Any())
                {
                    await InvokeFeaturesRecursivly(feature.FeatureDependencies, async currentFeature =>
                    {
                        invoker(feature);
                    });
                }

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
