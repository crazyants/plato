using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Internal.Models.Features;

namespace Plato.Internal.Features
{

    public interface IFeatureEventContext
    {

        IServiceProvider ServiceProvider { get; set; }

        IShellFeature Feature { get; set; }

        IDictionary<string, string> Errors { get; set; }

    }

    public class FeatureEventContext  : IFeatureEventContext
    {
        public IServiceProvider ServiceProvider { get; set; }

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
