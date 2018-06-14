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

    public class ShellDescriptorDocument : BaseDocument
    {

        private ShellDescriptor ShellDescriptor { get; set; }

    }

    public interface IShellFeatureManager
    {

        Task<IEnumerable<IFeatureInfo>> EnableFeaturesAsync(string[] featureIds);
            
        Task<IEnumerable<IFeatureInfo>> EnableFeaturesAsync(ShellDescriptor shellDescriptor,
            IEnumerable<IFeatureInfo> features);
        

    }

    public class ShellFeatureManager : IShellFeatureManager
    {

        private readonly IShellFeaturesStore _shellFeatureStore;
        private readonly ILogger<ShellFeatureManager> _logger;

        public ShellFeatureManager(
            IShellFeaturesStore shellFeatureStore,
            ILogger<ShellFeatureManager> logger)
        {
            _shellFeatureStore = shellFeatureStore;
            _logger = logger;
        }


        public async Task<IEnumerable<IFeatureInfo>> EnableFeaturesAsync(string[] featureIds)
        {

            var descriptor = new ShellDescriptor();
            foreach (var featureId in featureIds)
            {
                descriptor.Modules.Add(new ShellFeature(featureId));
            }


            var features = await _shellFeatureStore.SaveAsync(descriptor);


            return null;

        }


        public Task<IEnumerable<IFeatureInfo>> EnableFeaturesAsync(ShellDescriptor shellDescriptor, IEnumerable<IFeatureInfo> features)
        {
            throw new NotImplementedException();
        }
    }
}
