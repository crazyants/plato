using System.Collections.Generic;
using Plato.Follows.Models;

namespace Plato.Follows.Services
{

    public interface IFollowTypeProvider
    {
        IEnumerable<IFollowType> GetFollowTypes();
        
    }

}
