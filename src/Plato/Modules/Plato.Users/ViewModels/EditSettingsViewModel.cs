using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Users.ViewModels
{
    public class EditSettingsViewModel
    {

        public int Id { get; set; }

        [Required]
        public string TimeZone { get; set; }

        public bool ObserveDst { get; set; }
        
        public string Culture { get; set; }
        
        public IEnumerable<SelectListItem> AvailableTimeZones { get; set; }

    }
}
