using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Models.Users;

namespace Plato.Hosting.Web
{
    public interface IContextFacade
    {

        Task<User> GetAuthenticatedUser();

    }
}
