using System.Collections.Generic;
using Plato.Internal.Abstractions;

namespace Plato.Entities.Models
{

    public class EntityUri
    {

        public string Uri { get; set; }
        
        public bool External { get; set; }
        
    }

    public class EntityUris : Serializable
    {
        public IList<EntityUri> ImageUrls { get; set; }

        public IList<EntityUri> AnchorUrls { get; set; }

        public EntityUris()
        {
            this.ImageUrls = new List<EntityUri>();
            this.AnchorUrls = new List<EntityUri>();
        }

    }
}
