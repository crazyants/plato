using System.Collections.Generic;
using Plato.Follows.Models;

namespace Plato.Follows.Services
{
    public interface IFollowTypesManager
    {

        IEnumerable<IFollowType> GetFollowTypes();

    }

}
