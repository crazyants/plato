using System.Threading.Tasks;
using Plato.Internal.Models.Shell;

namespace Plato.Internal.Stores.Abstractions
{
    public interface IDictionaryStore<TModel> where TModel : class
    {

        Task<TModel> GetAsync();

        Task<TModel> SaveAsync(TModel model);

        Task<bool> DeleteAsync();

    }
}
