using Plato.Internal.Repositories;

namespace Plato.Reactions.Repositories
{
    public interface IReactionRepository<T> : IRepository<T> where T : class
    {

    }

}