using System.Collections.Generic;
using System.Linq;

namespace Plato.Internal.Assets.Abstractions
{
    
    public class AssetEnvironment
    {

        public Environment Environment { get; set; }

        public IList<Asset> Resources { get; set; }

        public AssetEnvironment(Environment env, IEnumerable<Asset> resources)
        {
            this.Environment = env;
            this.Resources = resources.ToList();
        }
    }
    
    public enum Environment
    {
        Development,
        Staging,
        Production
    }

}
