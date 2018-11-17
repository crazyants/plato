using System.Runtime.Serialization;

namespace Plato.WebApi.Models
{
    
    [DataContract]
    public class UserApiResult
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [DataMember(Name = "userName")]
        public string UserName { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "rank")]
        public int Rank { get; set; }

    }
    
}
