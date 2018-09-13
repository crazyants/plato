using System.ComponentModel.DataAnnotations;

namespace Plato.Users.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "email or username")]
        [StringLength(255, ErrorMessage = "The {0} must be at max {1} characters long.")]
        public string UserIdentifier { get; set; }
    }

}
