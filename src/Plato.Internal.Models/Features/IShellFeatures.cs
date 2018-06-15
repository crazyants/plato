using System.Collections.Generic;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Models.Features
{
    public interface IShellFeatures : ISerializable
    {
        IEnumerable<ShellFeature> Features { get; set; }

    }
}
