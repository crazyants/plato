using System.Collections.Generic;
using Plato.Stars.Models;
using Plato.Stars.Services;

namespace Plato.Issues.Star
{

    public class StarTypes : IStarTypeProvider
    {
        
        public static readonly StarType Issue =
            new StarType(
                "Issue",
                "Star",
                "Star this issue",
                "Unstar",
                "Delete star",
                "Login to star this issue",
                "You don't have permission to star issues");

        public IEnumerable<IStarType> GetFollowTypes()
        {
            return new[]
            {
                Issue
            };
        }

    }

}
