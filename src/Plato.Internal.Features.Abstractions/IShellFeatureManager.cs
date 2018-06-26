using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Features;

namespace Plato.Internal.Features.Abstractions
{
    public interface IShellFeatureManager
    {

        Task<IEnumerable<IFeatureEventContext>> EnableFeatureAsync(string featureId);

        Task<IEnumerable<IFeatureEventContext>> EnableFeaturesAsync(string[] featureIds);

        Task<IEnumerable<IFeatureEventContext>> DisableFeatureAsync(string featureId);
            
        Task<IEnumerable<IFeatureEventContext>> DisableFeaturesAsync(string[] featureIds);

    }

}
