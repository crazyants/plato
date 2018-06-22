using System.Threading.Tasks;
using Newtonsoft.Json;
using Plato.Internal.Abstractions;

namespace Plato.Users.Social.Models
{
    public class SocialLinks : Serializable
    {

        public string FacebookUrl { get; set; }

        public string TwitterUrl { get; set; }

        public string YouTubeUrl { get; set; }

    }

}
