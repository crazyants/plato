using System.Collections.Generic;
using Plato.Internal.Abstractions;

namespace Plato.Entities.Models
{

    public class EntityParticipant
    {

        public int UserId { get; set; }

        public string UserName { get; set; }

        public int Participations { get; set; }

    }
    
    public class EntityDetails : Serializable
    {

        public IEnumerable<EntityParticipant> Participants { get; set; }
        
    }

}
