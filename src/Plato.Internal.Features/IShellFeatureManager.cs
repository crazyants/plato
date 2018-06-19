using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Features;

namespace Plato.Internal.Features
{
    public interface IShellFeatureManager
    {

        Task<IEnumerable<IFeatureEventContext>> EnableFeaturesAsync(string[] featureIds);

        Task<IEnumerable<IFeatureEventContext>> DisableFeaturesAsync(string[] featureIds);

    }

}
