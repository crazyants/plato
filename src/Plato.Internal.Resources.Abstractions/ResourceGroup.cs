using System.Collections.Generic;

namespace Plato.Internal.Resources.Abstractions
{
    
    public class ResourceGroup
    {

        public DeploymentMode DeploymentMode { get; set; }

        public IEnumerable<Resource> Resources { get; set; }

        public ResourceGroup(DeploymentMode mode, IEnumerable<Resource> resources)
        {
            this.DeploymentMode = mode;
            this.Resources = resources;
        }
    }
    
    public enum DeploymentMode
    {
        Development,
        Staging,
        Production
    }

}
