using System.Collections.Generic;
using System.Runtime.Serialization;
using Plato.Internal.Abstractions;

namespace Plato.Entities.Models
{

    public class EntityUri
    {

        [DataMember(Name = "url")]
        public string Uri { get; set; }

        [DataMember(Name = "external")]
        public bool External { get; set; }
        
    }

    [DataContract]
    public class EntityUris : Serializable
    {
        [DataMember(Name = "imageUrls")]
        public IList<EntityUri> ImageUrls { get; set; }

        [DataMember(Name = "anchorUrls")]
        public IList<EntityUri> AnchorUrls { get; set; }

        public EntityUris()
        {
            this.ImageUrls = new List<EntityUri>();
            this.AnchorUrls = new List<EntityUri>();
        }

    }

}
