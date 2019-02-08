using System.ComponentModel.DataAnnotations;

namespace Plato.Users.reCAPTCHA2.ViewModels
{
    public class ReCaptchaSettingsViewModel
    {

        [Required]
        [StringLength(255)]
        [Display(Name = "site key")]
        public string SiteKey { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "secret")]
        public string Secret { get; set; }

    }
}
