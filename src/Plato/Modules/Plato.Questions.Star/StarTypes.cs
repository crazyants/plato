using System.Collections.Generic;
using Plato.Stars.Models;
using Plato.Stars.Services;

namespace Plato.Questions.Star
{

    public class StarTypes : IStarTypeProvider
    {
        
        public static readonly StarType Question =
            new StarType(
                "Question",
                "Star",
                "Star this question",
                "Unstar",
                "Delete star",
                "Login to star this question",
                "You don't have permission to star questions");

        public IEnumerable<IStarType> GetFollowTypes()
        {
            return new[]
            {
                Question
            };
        }

    }

}
