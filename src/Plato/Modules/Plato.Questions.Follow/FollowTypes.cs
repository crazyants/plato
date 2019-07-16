using System.Collections.Generic;
using Plato.Follows.Models;
using Plato.Follows.Services;

namespace Plato.Questions.Follow
{

    public class FollowTypes : IFollowTypeProvider
    {

        public static readonly FollowType Question =
            new FollowType(
                "Question",
                "Follow Question",
                "Follow this question to get notified when new answers are posted",
                "Unsubscribe",
                "You are following this question and will be notified when answers are posted, click to unsubscribe",
                "Login to follow this question",
                "You don't have permission to follow questions");

        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                Question
            };
        }

    }
}
