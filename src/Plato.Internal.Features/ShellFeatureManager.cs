using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Features;
using Plato.Internal.Models.Shell;
using Plato.Internal.Stores.Abstractions.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Internal.Features
{

    // Feature event handlers are executed in a temporary shell context 
    // This is necessary as the feature may not be enabled and as 
    // such the event handlers for the feature won't be registered with DI
    // For example we can't invoke the Installing or Installed events within
    // the main context as the feature is currently disabled within this context
    // so the IFeatureEventHandler provider for the feature has not been registered within DI.
    // ShellFeatureManager instead creates a temporary context consisting of a shell descriptor
    // with the features we want to enable or disable. IFeatureEventHandler can then be registered
    // within DI for the features we are enabling or disabling and the events can be invoked.

    public class ShellFeatureManager : IShellFeatureManager
    {

        private readonly IPlatoHost _platoHost;
        private readonly IShellDescriptorStore _shellDescriptorStore;
        private readonly IShellDescriptorFeatureManager _shellDescriptorFeatureManager;
        private readonly IRunningShellTable _runningShellTable;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ShellFeatureManager> _logger;
        private readonly IShellContextFactory _shellContextFactory;

        public ShellFeatureManager(
            IShellDescriptorStore shellDescriptorStore,
            IShellDescriptorFeatureManager shellDescriptorFeatureManager,
            IRunningShellTable runningShellTable, 
            IHttpContextAccessor httpContextAccessor,
            IShellContextFactory shellContextFactory,
            ILogger<ShellFeatureManager> logger,
            IPlatoHost platoHost)
        {
            _shellDescriptorStore = shellDescriptorStore;
            _shellDescriptorFeatureManager = shellDescriptorFeatureManager;
            _runningShellTable = runningShellTable;
            _httpContextAccessor = httpContextAccessor;
            _shellContextFactory = shellContextFactory;
            _platoHost = platoHost;
            _logger = logger;
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
            var featuresToInvoke = features.ToList();

            var contexts = new ConcurrentDictionary<string, IFeatureEventContext>();

            // Raise installing events for features
            InvokeFeatures(featuresToInvoke,
                (context, handler) =>
                {
                    // Ensure feature is not already enabled
                    if (!context.Feature.IsEnabled)
                    {
                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation($"{context.Feature.Id} InstallingAsync Event Raised");
                        }
                        handler.InstallingAsync(context);
                        contexts.AddOrUpdate(context.Feature.Id, context, (k, v) =>
                        {
                            v.Errors = context.Errors;
                            return v;
                        });
                    }

                }, contexts);

            // Did any event encounter errors?
            var hasErrors = contexts.Where(c => c.Value.Errors.Count > 0);

            // No errors update descriptor, raise InstalledAsync and recycle ShellContext
            if (!hasErrors.Any())
            {

                // Update descriptor within database
                var descriptor = await GetOrUpdateDescriptor(featureIds);
                var updatedDescriptor = await _shellDescriptorStore.SaveAsync(descriptor);

                // Raise Installed event
                InvokeFeatures(featuresToInvoke,
                    (context, handler) =>
                    { 
                        // Ensure feature is not already enabled
                        if (!context.Feature.IsEnabled)
                        {
                            if (_logger.IsEnabled(LogLevel.Information))
                            {
                                _logger.LogInformation($"{context.Feature.Id} InstalledAsync Event Raised");
                            }
                            handler.InstalledAsync(context);
                            contexts.AddOrUpdate(context.Feature.Id, context, (k, v) =>
                            {
                                v.Errors = context.Errors;
                                return v;
                            });
                        }
                    }, contexts);

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
            var featuresToInvoke = features.ToList();

            // Holds the results of all our event executation contexts
            var contexts = new ConcurrentDictionary<string, IFeatureEventContext>();

            // Raise Uninstalling events
            InvokeFeatures(featuresToInvoke,
                async (context, handler) =>
                {
                    // Ensure feature is enabled
                    if (context.Feature.IsEnabled)
                    {
                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation($"{context.Feature.Id} UninstallingAsync Event Raised");
                        }
                        await handler.UninstallingAsync(context);
                        contexts.AddOrUpdate(context.Feature.Id, context, (k, v) =>
                        {
                            v.Errors = context.Errors;
                            return v;
                        });
                    }
                }, contexts);

            // Did any event encounter errors?
            var hasErrors = contexts.Where(c => c.Value.Errors.Count > 0);

            // No errors update descriptor, raise InstalledAsync and recycle ShellContext
            if (!hasErrors.Any())
            {
                // Update features within data store
                var descriptor = await RemoveFeaturesFromCurrentDescriptor(featureIds);
                var updatedDescriptor = await _shellDescriptorStore.SaveAsync(descriptor);

                // Raise Uninstalled events for features
                InvokeFeatures(featuresToInvoke,
                    async (context, handler) =>
                    {
                        // Ensure feature is enabled
                        if (context.Feature.IsEnabled)
                        {
                            if (_logger.IsEnabled(LogLevel.Information))
                            {
                                _logger.LogInformation($"{context.Feature.Id} UninstalledAsync Event Raised");
                            }
                            await handler.UninstalledAsync(context);
                            contexts.AddOrUpdate(context.Feature.Id, context, (k, v) =>
                            {
                                v.Errors = context.Errors;
                                return v;
                            });
                        }
                    }, contexts);

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

        void InvokeFeatures(
            IList<IShellFeature> features,
            Action<IFeatureEventContext, IFeatureEventHandler> invoker,
            ConcurrentDictionary<string, IFeatureEventContext> contexts)
        {

            // Get setting before dispose
            var httpContext = _httpContextAccessor.HttpContext;
            var shellSettings = _runningShellTable.Match(httpContext);

            // Dispose shell
            RecycleShell();

            // Build descriptor to ensure correct feature event handlers are available within DI
            var descriptor = new ShellDescriptor();
            foreach (var feature in features)
            {
                descriptor.Modules.Add(new ShellModule(feature.Id));
            }
            
            // Create a new shell context
            using (var shellContext = _shellContextFactory.CreateDescribedContext(shellSettings, descriptor))
            {
                using (var scope = shellContext.ServiceProvider.CreateScope())
                {
                
                    var handlers = scope.ServiceProvider.GetServices<IFeatureEventHandler>();
                
                    foreach (var handler in handlers)
                    {
                        foreach (var feature in features)
                        {

                            var context = new FeatureEventContext()
                            {
                                Feature = feature,
                                ServiceProvider = scope.ServiceProvider
                            };
                          
                            try
                            {
                                invoker(context, handler);
                                if (context.Errors.Count > 0)
                                {
                                    foreach (var error in context.Errors)
                                    {
                                        _logger.LogError(error.Value, $"{context.Feature.Id} event handler threw an exception");
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, $"{context.Feature.Id} event handler threw an exception");
                                contexts.AddOrUpdate(context.Feature.Id, context, (k, v) =>
                                {
                                    v.Errors.Add(context.Feature.Id, e.Message);
                                    return v;
                                });
                            }
                          
                        }
                    }
                    
                }

            }

        }

        void DisposeShell(IShellSettings shellSettings)
        {
            _platoHost.DisposeShellContext(shellSettings);
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
