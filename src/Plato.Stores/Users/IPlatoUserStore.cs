using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Abstractions.Stores;
using Plato.Models.Users;

namespace Plato.Stores.Users
{
    public interface IPlatoUserStore : IStore<User>
    {
    }

}
