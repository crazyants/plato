using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Plato.Internal.Layout.ViewFeatures
{
    public interface IModularViewsFeatureProvider<TFeature> : IApplicationFeatureProvider<TFeature>
    {
    }
}
