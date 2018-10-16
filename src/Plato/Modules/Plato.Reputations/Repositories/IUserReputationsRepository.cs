using Plato.Internal.Repositories;

namespace Plato.Reputations.Repositories
{
    public interface IUserReputationsRepository<T> : IRepository<T> where T : class
    {
    }
}
