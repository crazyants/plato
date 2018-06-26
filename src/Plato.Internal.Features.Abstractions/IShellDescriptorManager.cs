using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Models.Features;
using Plato.Internal.Models.Shell;

namespace Plato.Internal.Features.Abstractions
{
    public interface IShellDescriptorManager
    {

        Task<IShellDescriptor> GetEnabledDescriptorAsync();

        Task<IEnumerable<IShellFeature>> GetEnabledFeaturesAsync();

        Task<IEnumerable<IShellFeature>> GetFeaturesAsync();

        Task<IEnumerable<IShellFeature>> GetFeaturesAsync(string[] featureIds);
            
        Task<IShellFeature> GetFeatureAsync(string featureId);

        Task<IEnumerable<IShellFeature>> GetFeatureDependenciesAsync(string featureId);

        Task<IEnumerable<IShellFeature>> GetDepdendentFeaturesAsync(string featureId);

    }

}
