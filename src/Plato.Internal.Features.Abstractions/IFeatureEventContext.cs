using System;
using System.Collections.Generic;
using Plato.Internal.Models.Features;

namespace Plato.Internal.Features.Abstractions
{
    public interface IFeatureEventContext
    {

        IServiceProvider ServiceProvider { get; set; }

        IShellFeature Feature { get; set; }

        IDictionary<string, string> Errors { get; set; }

    }

}
