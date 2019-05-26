using Plato.Internal.Repositories;

namespace Plato.Moderation.Repositories
{
    public interface IModeratorRepository<T> : IRepository2<T> where T : class
    {

    }

}
