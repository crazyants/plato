using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Modules.Abstractions.Features;

namespace Plato.Modules.Features
{
    public interface IFeatureManager
    {
        //FeatureDependencyNotificationHandler FeatureDependencyNotification { get; set; }

         Task<IEnumerable<FeatureDescriptor>> GetAvailableFeaturesAsync();

         Task<IEnumerable<FeatureDescriptor>> GetEnabledFeaturesAsync();

         Task<IEnumerable<FeatureDescriptor>> GetDisabledFeaturesAsync();

         Task<IEnumerable<string>> EnableFeaturesAsync(IEnumerable<string> featureIds);

         Task<IEnumerable<string>> EnableFeaturesAsync(IEnumerable<string> featureIds, bool force);

        Task<IEnumerable<string>> DisableFeaturesAsync(IEnumerable<string> featureIds);
                   
        Task<IEnumerable<string>> DisableFeaturesAsync(IEnumerable<string> featureIds, bool force);

        Task<IEnumerable<string>> GetDependentFeaturesAsync(string featureId);
    }
}
