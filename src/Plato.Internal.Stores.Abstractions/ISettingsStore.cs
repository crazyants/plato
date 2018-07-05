using System.Threading.Tasks;

namespace Plato.Internal.Stores.Abstractions
{
    public interface ISettingsStore<TModel> where TModel : class
    {

        Task<TModel> GetAsync();

        Task<TModel> SaveAsync(TModel model);

        Task<bool> DeleteAsync();

    }
}
