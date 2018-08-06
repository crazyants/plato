using System.Collections.Generic;

namespace Plato.Users.ViewModels
{
    public class EditSettingsViewModel
    {

        public int Id { get; set; }

        public double TimeZoneOffSet { get; set; }

        public string Culture { get; set; }
        
        public IDictionary<string, double> AvailableTimeZones { get; set; }

    }
}
