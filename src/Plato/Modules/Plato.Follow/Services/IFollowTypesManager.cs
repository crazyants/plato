using System.Collections.Generic;
using Plato.Follow.Models;

namespace Plato.Follow.Services
{
    public interface IFollowTypesManager
    {

        IEnumerable<IFollowType> GetFollowTypes();

    }

}
