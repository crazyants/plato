using System.Collections.Generic;
using Plato.Stars.Models;
using Plato.Stars.Services;

namespace Plato.Discuss.Star
{

    public class StarTypes : IStarTypeProvider
    {
        
        public static readonly StarType Topic =
            new StarType(
                "Topic",
                "Star",
                "Star this topic to quickly jump in anytime...",
                "Unstar",
                "You've starred this topic. Remove your star below...");

        public IEnumerable<IStarType> GetFollowTypes()
        {
            return new[]
            {
                Topic
            };
        }

    }

}
