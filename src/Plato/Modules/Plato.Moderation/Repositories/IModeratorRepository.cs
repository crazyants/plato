using Plato.Internal.Repositories;

namespace Plato.Moderation.Repositories
{
    public interface IModeratorRepository<T> : IRepository<T> where T : class
    {

    }

}
