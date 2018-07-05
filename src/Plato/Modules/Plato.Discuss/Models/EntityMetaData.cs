using System.Collections.Generic;
using Plato.Entities.Models;
using Plato.Internal.Abstractions;

namespace Plato.Discuss.Models
{

    public class Participant : EntityUser
    {
      
        public int Participations { get; set; }

    }


    public class EntityMetaData : Serializable
    {

        public string SomeNewValue { get; set; }
        
        public IEnumerable<Participant> Participants { get; set; } = new List<Participant>();


    }
}
