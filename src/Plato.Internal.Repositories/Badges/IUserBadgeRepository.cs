using Plato.Internal.Repositories;

namespace Plato.Internal.Repositories.Badges
{
    public interface IUserBadgeRepository<T> : IRepository2<T> where T : class
    {
    }
}
