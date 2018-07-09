using System.ComponentModel.DataAnnotations;

namespace Plato.Settings.ViewModels
{
    public class SiteSettingsViewModel
    {

        [Required]
        [StringLength(255)]
        [Display(Name = "site name")]
        public string SiteName { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "api key")]
        public string ApiKey { get; set; }


    }
}
