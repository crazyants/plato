using System;

namespace Plato.Internal.Repositories.Users
{
    public interface IUserSecretRepository<T> : IRepository<T> where T : class
    {
    }
}
