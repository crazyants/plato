using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Repositories.Users
{
    public interface IUserSecretRepository<T> : IRepository<T> where T : class
    {
    }
}
