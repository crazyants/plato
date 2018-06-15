using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Models.Features;
using Plato.Internal.Models.Shell;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Internal.Features
{

    public interface IShellDescriptorFeatureManager
    {
        Task<IEnumerable<ShellFeature>> GetFeaturesAsync();

    }

    public class ShellDescriptorFeatureManager : IShellDescriptorFeatureManager
    {

        private readonly IModuleManager _moduleManager;
        private readonly IShellDescriptor _shellDescriptor;

        public ShellDescriptorFeatureManager(
            IModuleManager moduleManager,
            IShellDescriptor shellDescriptor)
        {
            _moduleManager = moduleManager;
            _shellDescriptor = shellDescriptor;
        }
        
        public async Task<IEnumerable<ShellFeature>> GetFeaturesAsync()
        {

            var modules = await _moduleManager.LoadModulesAsync();

            var enabledFeatures = _shellDescriptor.Modules;
            
            var features = new List<ShellFeature>();

            foreach (var module in modules)
            {
                var isEnabled = enabledFeatures.FirstOrDefault(f => f.Id == module.Descriptor.Id) != null
                    ? true
                    : false;

                features.Add(new ShellFeature()
                {
                    Id = module.Descriptor.Id,
                    Name = module.Descriptor.Name,
                    Description = module.Descriptor.Description,
                    IsEnabled = isEnabled
                });
            }
            
            return features;

        }
    }
}
