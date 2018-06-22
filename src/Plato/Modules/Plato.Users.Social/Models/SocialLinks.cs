using Newtonsoft.Json;
using Plato.Internal.Abstractions;

namespace Plato.Users.Social.Models
{
    public class SocialLinks : ISerializable
    {

        public string FacebookUrl { get; set; }
        
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
