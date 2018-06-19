using System.Collections.Generic;
using Plato.Internal.Models.Features;

namespace Plato.Internal.Features
{

    public interface IFeatureEventContext
    {

        IShellFeature Feature { get; set; }

        IDictionary<string, string> Errors { get; set; }

    }

    public class FeatureEventContext  : IFeatureEventContext
    {

        public IShellFeature Feature { get; set; }

        public IDictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();

        public FeatureEventContext()
        {

        }

        public FeatureEventContext(IShellFeature feature)
        {
            this.Feature = feature;
        }

    }

}
