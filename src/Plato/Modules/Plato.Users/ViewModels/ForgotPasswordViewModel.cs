using System.ComponentModel.DataAnnotations;

namespace Plato.Users.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string UserIdentifier { get; set; }
    }

}
