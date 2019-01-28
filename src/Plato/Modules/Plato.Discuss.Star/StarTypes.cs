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
                "Star this topic to quickly join the discussion anytime...",
                "Unstar",
                "You've already starred this topic. Unsubscribe below...");

        public IEnumerable<IStarType> GetFollowTypes()
        {
            return new[]
            {
                Topic
            };
        }

    }
}
