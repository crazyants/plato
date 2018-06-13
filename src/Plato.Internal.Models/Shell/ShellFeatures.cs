using System.Collections.Generic;
using Newtonsoft.Json;

namespace Plato.Internal.Models.Shell
{
    public class ShellFeatures : IShellFeatures
    {
        public IEnumerable<ActiveFeature> Features { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
