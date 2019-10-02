using System.ComponentModel.DataAnnotations;
using Plato.Internal.Security.Attributes;

namespace Plato.SetUp.ViewModels
{
    public class SetUpViewModel
    {

        [Required, Display(Name = "site name")]
        public string SiteName { get; set; }

        [Required, Display(Name = "connection string")]
        public string ConnectionString { get; set; }

        public bool ConnectionStringPreset { get; set; }

        [Required, Display(Name = "table prefix")]
        public string TablePrefix { get; set; }
        
        public bool TablePrefixPreset { get; set; }

        [Required, Display(Name = "username"), StringLength(255, MinimumLength = 4)]
        [RegularExpression("[^\\n|^\\s|^,|^@]*", ErrorMessage = "The username cannot contain the @ or , characters.")]
        public string UserName { get; set; }

        [Required, EmailAddress, Display(Name = "email")]
        public string Email { get; set; }

        [Required, IdentityPasswordOptionsValidator, StringLength(100)]
        [DataType(DataType.Password), Display(Name = "password")]
        public string Password { get; set; }

        [Required, StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password), Display(Name = "password confirmation")]
        public string PasswordConfirmation { get; set; }
        
    }

}