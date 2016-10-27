using System.Threading.Tasks;
using Plato.Abstractions.Settings;

namespace Plato.Abstractions.Stores
{
    public interface ISettingsStore<T> where T : class
    {

        Task<T> GetAsync();

        Task<T> SaveAsync(T model);

        Task<bool> DeleteAsync();

    }
}
