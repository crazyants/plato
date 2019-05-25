using System;

namespace Plato.Internal.Repositories.Users
{
    public interface IUserSecretRepository<T> : IRepository2<T> where T : class
    {
    }
}
