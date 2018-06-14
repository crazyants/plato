using System.Collections.Generic;
using Newtonsoft.Json;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Models.Shell
{

    public interface IShellDescriptor : ISerializable
    {
        IList<ShellFeature> Modules { get; set; }

    }

    public class ShellDescriptor : IShellDescriptor
    {

        public IList<ShellFeature> Modules { get; set; } = new List<ShellFeature>();

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
