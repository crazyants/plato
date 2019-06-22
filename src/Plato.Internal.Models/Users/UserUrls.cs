using System.Runtime.Serialization;

namespace Plato.Internal.Models.Users
{
    public class UserUrls
    {
        
        [DataMember(Name = "profileUrl")]
        public string ProfileUrl { get; }
        
        [DataMember(Name = "getProfileUrl")]
        public string GetProfileUrl { get; }

        public UserUrls()
        {
        }

        public UserUrls(ISimpleUser user)
        {
            this.ProfileUrl = $"u/{user.Id}/{user.Alias}";
            this.GetProfileUrl = $"u/get/{user.Id}/{user.Alias}";
        }

    }

}
