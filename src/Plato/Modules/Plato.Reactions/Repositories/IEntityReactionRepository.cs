using Plato.Internal.Repositories;

namespace Plato.Reactions.Repositories
{
    public interface IEntityReactionRepository<T> : IRepository<T> where T : class
    {

    }

}