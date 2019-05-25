using Plato.Internal.Repositories;

namespace Plato.Media.Repositories
{

    public interface IMediaRepository<T> : IRepository2<T> where T : class
    {

    }

}
