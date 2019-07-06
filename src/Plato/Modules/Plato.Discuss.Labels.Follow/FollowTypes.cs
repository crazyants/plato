using System.Collections.Generic;
using Plato.Follows.Models;
using Plato.Follows.Services;

namespace Plato.Discuss.Labels.Follow
{

    public class FollowTypes : IFollowTypeProvider
    {
        
        public static readonly FollowType Label =
            new FollowType("DiscussLabel",
                "Follow Label",
                "Follow this label to get notified when new topics are posted with this label",
                "Unsubscribe",
                "You are following this label and will be notified when topics are posted with this label, click to unsubscribe");

        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                Label
            };
        }

    }
}
