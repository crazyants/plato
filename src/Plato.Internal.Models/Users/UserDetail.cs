using System;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Models.Users
{
    
    public class UserDetail : Serializable
    {

        public SettingsData Settings { get; set; } = new SettingsData();

        public ProfileData Profile { get; set; } = new ProfileData();

        //public bool IsEmailConfirmed { get; set; }

        //public int ModifiedUserId { get; set; }

        //public DateTime? ModifiedDate { get; set; }
        
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
