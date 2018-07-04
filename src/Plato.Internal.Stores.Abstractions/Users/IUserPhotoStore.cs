using System.Threading.Tasks;

namespace Plato.Internal.Stores.Abstractions.Users
{
    public interface IUserPhotoStore<TModel> : IStore<TModel> where TModel : class
    {
        Task<TModel> GetByUserIdAsync(int userId);
    }
}