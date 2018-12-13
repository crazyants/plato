using System.Threading.Tasks;

namespace Plato.Internal.Abstractions
{
    public interface ISerializable
    {

        string Serialize();

        Task<string> SerializeAsync();

        T Deserialize<T>(string data);

        Task<T> DeserializeAsync<T>(string data);
        
    }
    
}
