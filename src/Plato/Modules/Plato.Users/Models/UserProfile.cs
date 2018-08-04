using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Users.Models
{
    
    public class UserProfile : User
    {
        // UserProfile is simply a marker class so we can use
        // a separate view provider for the front-end profile pages
        // This class should not contain any code
    }

    public class UserDetail : Serializable
    {

        public SettingsData Settings { get; set; } = new SettingsData();

        public ProfileData Profile { get; set; } = new ProfileData();

    }

    public class SettingsData : Serializable
    {
        public double TimeZoneOffset { get; set; } = 0.0;

        public string Culture { get; set; }

    }

    public class ProfileData : Serializable
    {

        public string Location { get; set; }

        public string Bio { get; set; }

    }


}
