using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Users.ViewModels
{
    public class EditSettingsViewModel
    {

        public int Id { get; set; }

        public double TimeZoneOffSet { get; set; }

        public string Culture { get; set; }
        
        public IEnumerable<SelectListItem> AvailableTimeZones { get; set; }

    }
}
