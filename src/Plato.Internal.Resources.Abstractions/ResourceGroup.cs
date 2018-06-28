using System.Collections.Generic;
using System.Linq;

namespace Plato.Internal.Resources.Abstractions
{
    
    public class ResourceGroup
    {

        public Environment Environment { get; set; }

        public IList<Resource> Resources { get; set; }

        public ResourceGroup(Environment env, IEnumerable<Resource> resources)
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
