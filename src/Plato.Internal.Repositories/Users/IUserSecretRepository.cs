using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Internal.Repositories.Users
{
    public interface IUserSecretRepository<T> : IRepository<T> where T : class
    {
    }
}
