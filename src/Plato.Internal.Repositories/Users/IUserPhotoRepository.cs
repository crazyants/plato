using System.Threading.Tasks;

namespace Plato.Internal.Repositories.Users
{
    public interface IUserPhotoRepository<T> : IRepository2<T> where T : class
    {

        Task<T> SelectByUserIdAsync(int userId);

    }
}
