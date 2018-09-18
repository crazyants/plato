using System;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Features;
using Plato.Internal.Stores.Abstractions.Shell;

namespace Plato.Internal.Features
{

    public class FeatureFacade : IFeatureFacade
    {
        
        private readonly IShellFeatureStore<ShellFeature> _shellFeatureStore;

        public FeatureFacade(IShellFeatureStore<ShellFeature> shellFeatureStore)
        {
            _shellFeatureStore = shellFeatureStore;
        }
 
        public async Task<IShellFeature> GetFeatureByIdAsync(string moduleId)
        {
            var features = await _shellFeatureStore.SelectFeatures();
            return features?.FirstOrDefault(f => f.ModuleId.Equals(moduleId, StringComparison.OrdinalIgnoreCase));
        }

    }

}
