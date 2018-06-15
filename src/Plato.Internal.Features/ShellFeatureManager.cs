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
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Internal.Features
{

    public interface IShellFeatureManager
    {

        Task<IEnumerable<IShellFeature>> EnableFeaturesAsync(string[] featureIds);

        Task<IEnumerable<IShellFeature>> DisableFeaturesAsync(string[] featureIds);
        
        Task<IEnumerable<IShellFeature>> EnableFeaturesAsync(
            ShellDescriptor shellDescriptor,
            IEnumerable<IShellFeature> features);
        

    }

    public class ShellFeatureManager : IShellFeatureManager
    {

        private readonly IPlatoHost _platoHost;
        private readonly IShellDescriptorStore _shellDescriptorStore;
        private readonly IShellDescriptorFeatureManager _shellDescriptorFeatureManager;
        private readonly IServiceCollection _applicationServices;
        private readonly IRunningShellTable _runningShellTable;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ShellFeatureManager> _logger;
        
        public ShellFeatureManager(
            IShellDescriptorStore shellDescriptorStore,
            IShellDescriptorFeatureManager shellDescriptorFeatureManager,
            ILogger<ShellFeatureManager> logger,
            IServiceCollection applicationServices,
            IRunningShellTable runningShellTable, 
            IHttpContextAccessor httpContextAccessor, IPlatoHost platoHost)
        {
            _shellDescriptorStore = shellDescriptorStore;
            _shellDescriptorFeatureManager = shellDescriptorFeatureManager;
            _logger = logger;
            _applicationServices = applicationServices;
            _runningShellTable = runningShellTable;
            _httpContextAccessor = httpContextAccessor;
            _platoHost = platoHost;
        }


        public async Task<IEnumerable<IShellFeature>> EnableFeaturesAsync(string[] featureIds)
        {


            var descriptor =
                await _shellDescriptorStore.GetAsync()
                ?? new ShellDescriptor();

            foreach (var featureId in featureIds)
            {
                descriptor.Modules.Add(new ShellModule(featureId));
            }


            // Update features within data store
            var features = await _shellDescriptorStore.SaveAsync(descriptor);
            
            // dispose current shell context
            RecycleShell();

            return null;

        }

        public async Task<IEnumerable<IShellFeature>> DisableFeaturesAsync(string[] featureIds)
        {

            var enabledFeatures = await _shellDescriptorFeatureManager.GetEnabledFeaturesAsync();

            var descriptor = new ShellDescriptor();
            foreach (var feature in enabledFeatures)
            {
                var diable = featureIds.Any(f => f.Equals(feature.Id, StringComparison.InvariantCultureIgnoreCase));
                if (!diable)
                {
                    descriptor.Modules.Add(new ShellModule(feature.Id));
                }

            }

            // Update features within data store
            var features = await _shellDescriptorStore.SaveAsync(descriptor);
            
            // dispose current shell context
            RecycleShell();

            return null;

        }

        public Task<IEnumerable<IShellFeature>> EnableFeaturesAsync(
            ShellDescriptor shellDescriptor,
            IEnumerable<IShellFeature> features)
        {
            throw new NotImplementedException();
        }


        void RecycleShell()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var shellSettings = _runningShellTable.Match(httpContext);
            _platoHost.RecycleShellContext(shellSettings);
        }



    }
}
