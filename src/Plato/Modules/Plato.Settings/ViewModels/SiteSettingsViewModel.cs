using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Settings.ViewModels
{
    public class SiteSettingsViewModel
    {

        [Required, StringLength(255), Display(Name = "site name")]
        public string SiteName { get; set; }

        [Required, Display(Name = "time zone")]
        public string TimeZone { get; set; }
        
        public string DateTimeFormat { get; set; }

        public string Culture { get; set; }

        public string Theme { get; set; }
        
        public string HomeRoute { get; set; }

        [Required, StringLength(255), Display(Name = "default alias")]
        public string HomeAlias { get; set; } = "support";

        public IEnumerable<SelectListItem> AvailableTimeZones { get; set; }

        public IEnumerable<SelectListItem> AvailableDateTimeFormat { get; set; }

        public IEnumerable<SelectListItem> AvailableCultures { get; set; }

        public IEnumerable<SelectListItem> AvailableThemes { get; set; }

        public IEnumerable<SelectListItem> AvailableHomeRoutes { get; set; }

    }

}
