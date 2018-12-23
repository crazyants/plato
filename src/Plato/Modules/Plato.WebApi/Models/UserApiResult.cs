using System.Runtime.Serialization;
using Plato.Internal.Models.Users;

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

        [DataMember(Name = "avatar")]
        public UserAvatar Avatar { get; set; }

    }
    
}
