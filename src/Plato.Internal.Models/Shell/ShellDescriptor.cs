using System.Collections.Generic;
using Newtonsoft.Json;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Models.Shell
{

    public interface IShellDescriptor : ISerializable
    {
        IList<ShellModule> Modules { get; set; }

    }

    public class ShellDescriptor : IShellDescriptor
    {

        public IList<ShellModule> Modules { get; set; } = new List<ShellModule>();

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
