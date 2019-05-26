using Plato.Internal.Repositories;

namespace Plato.Internal.Repositories.Badges
{
    public interface IUserBadgeRepository<T> : IRepository<T> where T : class
    {
    }
}
