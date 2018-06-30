using System.Collections.Generic;
using Plato.Internal.Abstractions;

namespace Plato.Discuss.Models
{

    public class Participant
    {

        public int UserId { get; set; }

        public string Test { get; set; }
        
        public string UserName { get; set; }

        public int Participations { get; set; }

    }


    public class TopicDetails : Serializable
    {

        public string SomeNewValue { get; set; }
        
        public IEnumerable<Participant> Users { get; set; }


    }
}
