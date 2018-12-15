using System.Collections.Generic;
using Plato.Follow.Models;
using Plato.Follow.Services;

namespace Plato.Follow.Tags
{

    public class FollowTypes : IFollowTypeProvider
    {
        
        public static readonly FollowType Tag =
            new FollowType("Tag",
                "Follow Tag",
                "Folow this tag to get notified when new content is posted with this tag.",
                "Unsubscribe",
                "You are already following this tag. Unsubscribe below...");

        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                Tag
            };
        }

    }
}
