using System.Collections.Generic;
using Plato.Follow.Models;
using Plato.Follow.Services;

namespace Plato.Follow.Users
{

    public class FollowTypes : IFollowTypeProvider
    {

        public static readonly FollowType User =
            new FollowType("User",
                "Follow User",
                "Folow this user to get notified when they post new content.",
                "Unsubscribe",
                "You are already following this user.");

        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                User
            };
        }

    }
}
