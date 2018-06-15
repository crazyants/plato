using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Models;
using Plato.Internal.Features.Models;
using Plato.Internal.Models.Shell;
using Plato.Internal.Stores.Abstractions.Shell;

namespace Plato.Internal.Features
{

    public interface IShellFeatureManager
    {

        Task<IEnumerable<IFeatureInfo>> EnableFeaturesAsync(string[] featureIds);
            
        Task<IEnumerable<IFeatureInfo>> EnableFeaturesAsync(ShellDescriptor shellDescriptor,
            IEnumerable<IFeatureInfo> features);
        

    }

    public class ShellFeatureManager : IShellFeatureManager
    {

        private readonly IShellDescriptorStore _shellDescriptorStore;
        private readonly ILogger<ShellFeatureManager> _logger;

        public ShellFeatureManager(
            IShellDescriptorStore shellDescriptorStore,
            ILogger<ShellFeatureManager> logger)
        {
            _shellDescriptorStore = shellDescriptorStore;
            _logger = logger;
        }


        public async Task<IEnumerable<IFeatureInfo>> EnableFeaturesAsync(string[] featureIds)
        {

            var descriptor =
                await _shellDescriptorStore.GetAsync()
                ?? new ShellDescriptor();

            foreach (var featureId in featureIds)
            {
                descriptor.Modules.Add(new ShellModule(featureId));
            }
            
            var features = await _shellDescriptorStore.SaveAsync(descriptor);
            
            return null;

        }


        public Task<IEnumerable<IFeatureInfo>> EnableFeaturesAsync(ShellDescriptor shellDescriptor, IEnumerable<IFeatureInfo> features)
        {
            throw new NotImplementedException();
        }
    }
}
