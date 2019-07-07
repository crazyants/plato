using System.Collections.Generic;
using Plato.Stars.Models;
using Plato.Stars.Services;

namespace Plato.Ideas.Star
{

    public class StarTypes : IStarTypeProvider
    {
        
        public static readonly StarType Idea =
            new StarType(
                "Idea",
                "Star",
                "Star this idea",
                "Unstar",
                "Delete star",
                "Login to star this idea",
                "You don't have permission to star ideas");

        public IEnumerable<IStarType> GetFollowTypes()
        {
            return new[]
            {
                Idea
            };
        }

    }

}
