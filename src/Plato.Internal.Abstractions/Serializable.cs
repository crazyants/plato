using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Plato.Internal.Abstractions
{
    public class Serializable : ISerializable
    {

        public virtual string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public virtual async Task<string> SerializeAsync()
        {
            return await Task.Factory.StartNew(() => JsonConvert.SerializeObject(this));
        }

        public virtual T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public virtual async Task<T> DeserializeAsync<T>(string data)
        {
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(data));
        }

    }

}
