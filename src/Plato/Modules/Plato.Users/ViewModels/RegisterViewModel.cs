using Plato.Internal.Security.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Plato.Users.ViewModels
{
    public class RegisterViewModel
    {

        [Required, Display(Name = "username"), StringLength(255, MinimumLength = 4)]
        [RegularExpression("[^\\n|^\\s|^,|^@]*", ErrorMessage = "The username cannot contain the @ or , characters.")]
        public string UserName { get; set; }

        [Required, EmailAddress, Display(Name = "email")]
        public string Email { get; set; }

        [Required, IdentityPasswordOptionsValidator, StringLength(100)]
        [DataType(DataType.Password), Display(Name = "password")]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password")]
        [Display(Name = "password confirmation")]
        public string ConfirmPassword { get; set; }

    }

}