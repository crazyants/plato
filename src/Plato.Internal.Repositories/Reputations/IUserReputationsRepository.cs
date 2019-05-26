using Plato.Internal.Repositories;

namespace Plato.Internal.Repositories.Reputations
{
    public interface IUserReputationsRepository<T> : IRepository<T> where T : class
    {
    }
}
