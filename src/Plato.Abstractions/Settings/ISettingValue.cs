namespace Plato.Abstractions.Settings
{
    public interface ISettingValue
    {

        string Serialize();

        T Deserialize<T>(string json);
    }
}
