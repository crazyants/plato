using System.Collections.Generic;
using Newtonsoft.Json;
using Plato.Internal.Models.Features;

namespace Plato.Internal.Models.Features
{
    public class ShellFeatures : IShellFeatures
    {
        public IEnumerable<ShellFeature> Features { get; set; } = new List<ShellFeature>();

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
