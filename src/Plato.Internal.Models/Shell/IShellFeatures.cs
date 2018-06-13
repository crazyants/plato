using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Models.Shell
{
    public interface IShellFeatures : ISerializable
    {
        IEnumerable<FeatureInfo> Features { get; set; }

    }
}
