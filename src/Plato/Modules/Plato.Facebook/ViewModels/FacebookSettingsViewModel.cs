using System.ComponentModel.DataAnnotations;

namespace Plato.Facebook.ViewModels
{
    public class FacebookSettingsViewModel
    {

        [Required]
        [StringLength(255)]
        [Display(Name = "app id")]
        public string AppId { get; set; }

    }
}
