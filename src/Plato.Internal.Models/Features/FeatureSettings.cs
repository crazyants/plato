using Plato.Internal.Abstractions;

namespace Plato.Internal.Models.Features
{
    
    public class FeatureSettings : Serializable, IFeatureSettings
    {

        public string Title { get; set; }

        public string Description { get; set; }

        public string IconCss { get; set; }

    }

}
