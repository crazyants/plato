using System;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Features;
using Plato.Internal.Models.Shell;
using Plato.Internal.Stores.Abstractions.Shell;

namespace Plato.Internal.Features
{
  
    public class FeatureFacade : IFeatureFacade
    {

        private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly IShellFeatureStore<ShellFeature> _shellFeatureStore;

        public FeatureFacade(
            IShellDescriptorManager shellDescriptorManager,
            IShellFeatureStore<ShellFeature> shellFeatureStore)
        {
            _shellDescriptorManager = shellDescriptorManager;
            _shellFeatureStore = shellFeatureStore;
        }
        
        public async Task<IShellModule> GetModuleByIdAsync(string moduleId)
        {
            var descriptor = await _shellDescriptorManager.GetEnabledDescriptorAsync();
            return descriptor?.Modules?.FirstOrDefault(m =>
                m.ModuleId.Equals(moduleId, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IShellFeature> GetFeatureByIdAsync(string moduleId)
        {
            var features = await _shellFeatureStore.SelectFeatures();
            return features?.FirstOrDefault(f => f.ModuleId.Equals(moduleId, StringComparison.OrdinalIgnoreCase));
        }
    }

}
