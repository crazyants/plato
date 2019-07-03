using System.Collections.Generic;
using Plato.Internal.Abstractions;

namespace Plato.Follows.Models
{
    
    public class FollowState : Serializable
    {
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
