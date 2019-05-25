using Plato.Internal.Repositories;

namespace Plato.Email.Repositories
{
    public interface IEmailRepository<T> : IRepository2<T> where T : class
    {

    }

}
