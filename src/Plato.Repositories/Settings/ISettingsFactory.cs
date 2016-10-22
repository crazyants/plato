using Plato.Abstractions.Settings;

namespace Plato.Repositories.Settings
{
    public interface ISettingsFactory
    {
        
        T GetSettings<T>(string key);

        T UpdateSettings<T>(string key, ISettingValue value);

        T InsertSettings<T>(string key, ISettingValue value);

    }
}
