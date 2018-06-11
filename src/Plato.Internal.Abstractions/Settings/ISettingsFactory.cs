using Plato.Internal.Abstractions.Settings;
using System.Threading.Tasks;

namespace Plato.Internal.Abstractions.Settings
{
    public interface ISettingsFactory
    {
        
        Task<T> GetSettingsAsync<T>(string key);

        Task<T> UpdateSettingsAsync<T>(string key, ISettingValue value);
        
    }
}
