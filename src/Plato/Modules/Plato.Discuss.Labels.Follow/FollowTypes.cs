using System.Collections.Generic;
using Plato.Follows.Models;
using Plato.Follows.Services;

namespace Plato.Discuss.Labels.Follow
{

    public class FollowTypes : IFollowTypeProvider
    {
        
        public static readonly FollowType Label =
            new FollowType("Label",
                "Follow Label",
                "Folow this label to get notified when new content is posted with this label.",
                "Unsubscribe",
                "You are already following this label.");

        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                Label
            };
        }

    }
}
