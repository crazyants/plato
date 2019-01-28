using System.Collections.Generic;
using Plato.Stars.Models;

namespace Plato.Stars.Services
{

    public interface IStarTypeProvider
    {
        IEnumerable<IStarType> GetFollowTypes();
        
    }

}
