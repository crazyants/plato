using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Features.Models;
using Plato.Internal.Shell.Abstractions.Models;
using Plato.Internal.Stores.Abstractions.Shell;

namespace Plato.Internal.Features
{

    public interface IShellFeatureManager
    {

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
        
        public Task<IEnumerable<IFeatureInfo>> EnableFeaturesAsync(ShellDescriptor shellDescriptor, IEnumerable<IFeatureInfo> features)
        {
            throw new NotImplementedException();
        }
    }
}
