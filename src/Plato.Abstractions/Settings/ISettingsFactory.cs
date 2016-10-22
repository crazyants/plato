using Plato.Abstractions.Settings;
using System.Threading.Tasks;

namespace Plato.Abstractions.Settings
{
    public interface ISettingsFactory
    {
        
        Task<T> GetSettingsAsync<T>(string key);

        Task<T> UpdateSettingsAsync<T>(string key, ISettingValue value);
        
    }
}
