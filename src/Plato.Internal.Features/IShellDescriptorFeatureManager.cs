using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Models.Features;

namespace Plato.Internal.Features
{
    public interface IShellDescriptorFeatureManager
    {

        Task<IEnumerable<IShellFeature>> GetEnabledFeaturesAsync();

        Task<IEnumerable<IShellFeature>> GetFeaturesAsync();

        Task<IEnumerable<IShellFeature>> GetFeatureDependenciesAsync(string featureId);

        Task<IEnumerable<IShellFeature>> GetDepdendentFeaturesAsync(string featureId);

    }

}
