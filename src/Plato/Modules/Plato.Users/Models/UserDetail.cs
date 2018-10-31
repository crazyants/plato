using System.Collections.Generic;
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
        private IEnumerable<SettingData> Settings { get; set; } = new List<SettingData>();

    }

    public class SettingData : Serializable
    {

        public string Key { get; set; }

        public object Value { get; set; }

    }

    public class ProfileData : Serializable
    {

        public string Location { get; set; }

        public string Bio { get; set; }

        public string Url { get; set; }

    }


}
