using Plato.Internal.Repositories;

namespace Plato.Entities.Repositories
{
    public interface IEntityRepository<T> : IRepository<T> where T : class
    {

    }

}
