using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace Plato.Internal.Models.Users
{
    [DataContract]
    public class UserAvatar
    {

        [DataMember(Name = "url")]
        public string Url { get; }

        public bool HasAvatar { get; }

        public UserAvatar()
        {
        }

        public UserAvatar(ISimpleUser user)
        {

            if (user.Id <= 0)
            {
                Url = "/images/photo.png";
                return;
            }

            // If we have a photo Url use that
            if (!string.IsNullOrEmpty(user.PhotoUrl))
            {
                Url = user.PhotoUrl;
                HasAvatar = true;
                return;
            }

            var letter = user.DisplayName != null 
                ? user.DisplayName.ToLower().Substring(0, 1) 
                : "-";

            // Else fallback to our letter service
            Url = $"/u/l/{letter}/{user.PhotoColor}";

        }

    }
}
