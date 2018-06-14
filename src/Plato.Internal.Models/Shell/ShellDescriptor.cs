using System.Collections.Generic;
using Newtonsoft.Json;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Models.Shell
{
    public class ShellDescriptor : ISerializable
    {

        public IList<ShellFeature> Modules { get; set; } = new List<ShellFeature>();

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
