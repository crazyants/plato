using System.Collections.Generic;
using System.Runtime.Serialization;
using Plato.Internal.Abstractions;

namespace Plato.Follows.Models
{
    
    [DataContract]
    public class FollowState : Serializable
    {

        [DataMember(Name = "followsSent")]
        public ICollection<string> FollowsSent { get; }
        
        public FollowState()
        {
            FollowsSent = new List<string>();
        }

        public void AddSent(string name)
        {
            FollowsSent.Add(name);
        }

    }
    
}
