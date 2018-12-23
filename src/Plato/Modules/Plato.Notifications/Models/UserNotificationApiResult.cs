using System.Runtime.Serialization;
using Plato.Internal.Models.Users;
using Plato.WebApi.Models;

namespace Plato.Notifications.Models
{

    [DataContract]
    public class UserNotificationApiResult
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "to")]
        public UserApiResult To { get; set; }

        [DataMember(Name = "from")]
        public UserApiResult From { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "date")]
        public IFriendlyDate Date { get; set; }

        [DataMember(Name = "avatar")]
        public UserAvatar Avatar { get; set; }

    }
    
}
