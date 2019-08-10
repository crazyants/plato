using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Plato.SetUp.ViewModels
{
    public class SetUpViewModel
    {
     
        [Required]
        public string SiteName { get; set; }

        //[Required]
        //public string DatabaseProvider { get; set; }

        //public bool DatabaseProviderPreset { get; set; }

        [Required]
        public string ConnectionString { get; set; }

        public bool ConnectionStringPreset { get; set; }

        public string TablePrefix { get; set; }
        
        public bool TablePrefixPreset { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password)]
        public string PasswordConfirmation { get; set; }
        
    }

}