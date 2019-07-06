using System.Collections.Generic;
using Plato.Stars.Models;
using Plato.Stars.Services;

namespace Plato.Docs.Star
{

    public class StarTypes : IStarTypeProvider
    {
        
        public static readonly StarType Doc =
            new StarType(
                "Doc",
                "Star",
                "Star this doc",
                "Unstar",
                "Delete star",
                "Login to star this doc",
                "You don't have permission to star docs");

        public IEnumerable<IStarType> GetFollowTypes()
        {
            return new[]
            {
                Doc
            };
        }

    }

}
