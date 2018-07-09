using System.ComponentModel.DataAnnotations;

namespace Plato.Settings.ViewModels
{
    public class SiteSettingsViewModel
    {

        [Required]
        public string SiteName { get; set; }

    }
}
