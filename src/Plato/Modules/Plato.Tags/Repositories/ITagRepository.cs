using Plato.Internal.Repositories;

namespace Plato.Tags.Repositories
{

    public interface ITagRepository<T> : IRepository<T> where T : class
    {
    }

}
