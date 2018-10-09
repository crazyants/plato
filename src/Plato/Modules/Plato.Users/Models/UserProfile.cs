using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Users.Models
{
    
    public class UserDetail : Serializable
    {

        public SettingsData Settings { get; set; } = new SettingsData();

        public ProfileData Profile { get; set; } = new ProfileData();

    }

    public class SettingsData : Serializable
    {
        //public string TimeZone { get; set; }

        //public bool ObserveDst { get; set; }

        //public string Culture { get; set; }

    }

    public class ProfileData : Serializable
    {

        public string Location { get; set; }

        public string Bio { get; set; }

        public string Url { get; set; }
        
    }


}
