using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Hosting.Web
{
    public interface IContextFacade
    {

        Task<User> GetAuthenticatedUserAsync();

    }
}
