using System.Collections.Generic;
using Plato.Internal.Abstractions;

namespace Plato.Follows.Models
{
    
    public class FollowState : Serializable
    {
        public ICollection<FollowSent> FollowsSent { get; set; }
        
        public FollowState()
        {
            FollowsSent = new List<FollowSent>();
        }

        public void AddSent(string name)
        {
            FollowsSent.Add(new FollowSent(name, true));
        }

    }

    public class FollowSent
    {
        public string Name { get; set; }

        public bool Sent { get; set; }

        public FollowSent(string name, bool sent)
        {
            Name = name;
            Sent = sent;
        }

    }

}
