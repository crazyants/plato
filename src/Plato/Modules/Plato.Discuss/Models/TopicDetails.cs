using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Abstractions;

namespace Plato.Discuss.Models
{

    public class Participant
    {

        public int UserId { get; set; }

        public string UserName { get; set; }

        public int Participations { get; set; }

    }


    public class TopicDetails : Serializable
    {

        public IEnumerable<Participant> Participants { get; set; }

    }
}
