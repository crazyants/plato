using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Features;
using Plato.Internal.Models.Shell;

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

}
